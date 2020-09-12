using System.Collections.Generic;
using Vector;
using Chains;
using Core;
using Core.Items;
using System.Linq;
using Core.Targeting;
using Core.Behaviors;

// Hello World! program
namespace Hopper
{
    class Hello
    {

        static void Main(string[] args)
        {
            System.Console.WriteLine("\n ------ Definition + Instantiation Demo ------ \n");

            World world = new World
            {
                m_grid = new GridManager(5, 5),
                m_state = new WorldStateManager()
            };
            System.Console.WriteLine("Created world");

            int testStatusId = Statused.RegisterStatus("test", TestStatusTinkerStuff.status, 1);

            var playerFactory = new EntityFactory<Player>();
            playerFactory.AddBehavior<Attackable>();
            playerFactory.AddBehavior<Attacking>();
            playerFactory.AddBehavior<Displaceable>();
            playerFactory.AddBehavior<Moving>();
            playerFactory.AddBehavior<Pushable>();
            playerFactory.AddBehavior<Statused>();
            System.Console.WriteLine("Set up playerFactory");

            Acting.Config playerActingConf = new Acting.Config
            {
                DoAction = Algos.SimpleAlgo
            };

            playerFactory.AddBehavior<Acting>(playerActingConf);
            playerFactory.RetouchAndSave(Core.Retouchers.Skip.EmptyAttack);

            // this one's for the equip demo
            playerFactory.RetouchAndSave(Core.Retouchers.Equip.OnDisplace);


            var enemyFactory = new EntityFactory<Entity>();
            enemyFactory.AddBehavior<Attackable>();
            enemyFactory.AddBehavior<Attacking>();
            enemyFactory.AddBehavior<Displaceable>();
            enemyFactory.AddBehavior<Moving>();
            enemyFactory.AddBehavior<Pushable>();


            Acting.Config enemyActingConf = new Acting.Config
            {
                DoAction = Algos.EnemyAlgo
            };

            enemyFactory.AddBehavior<Acting>(enemyActingConf);


            var attackAction = new BehaviorAction<Attacking>();
            var moveAction = new BehaviorAction<Moving>();
            CompositeAction attackMoveAction = new CompositeAction(
                new Action[] { attackAction, moveAction }
            );

            StepData[] stepData =
            {
                new StepData { action = null },
                new StepData { action = attackMoveAction, movs = Movs.Basic }
            };

            var sequenceConfig = new Sequential.Config(stepData);

            enemyFactory.AddBehavior<Sequential>(sequenceConfig);
            System.Console.WriteLine("Set up enemyFactory");

            Entity player = playerFactory.Instantiate();
            System.Console.WriteLine("Instantiated Player");

            Entity enemy = enemyFactory.Instantiate();
            System.Console.WriteLine("Instantiated Enemy");

            enemy.Init(new IntVector2(1, 2), world);
            world.m_state.AddEntity(enemy);
            world.m_grid.Reset(enemy);
            System.Console.WriteLine("Enemy set in world");

            player.Init(new IntVector2(1, 1), world);
            world.m_state.AddPlayer(player);
            world.m_grid.Reset(player);
            System.Console.WriteLine("Player set in world");

            var playerNextAction = attackMoveAction.Copy();
            playerNextAction.direction = new IntVector2(0, 1);
            player.GetBehavior<Acting>().NextAction = playerNextAction;
            System.Console.WriteLine("Set player action");
            System.Console.WriteLine("\n ------ Modifier Demo ------ \n");

            var mod = new StatModifier("attack", new Attacking.Attack { damage = 1 });
            var attack = (Attacking.Attack)player.StatManager.GetFile("attack");
            System.Console.WriteLine("Attack damage:{0}", attack.damage);
            System.Console.WriteLine("Attack pierce:{0}", attack.pierce);
            System.Console.WriteLine("Adding modifier");
            player.StatManager.AddModifier(mod);
            // attack = (Attacking.Attack)player.m_statManager.GetFile("attack");
            System.Console.WriteLine("Attack damage:{0}", attack.damage);
            System.Console.WriteLine("Attack pierce:{0}", attack.pierce);
            System.Console.WriteLine("Removing modifier");
            player.StatManager.RemoveModifier(mod);
            System.Console.WriteLine("Attack damage:{0}", attack.damage);
            System.Console.WriteLine("Attack pierce:{0}", attack.pierce);

            var mod2 = new ChainModifier("attack", new Chains.EvHandler<StatEvent>(
                (StatEvent ev) =>
                {
                    System.Console.WriteLine("Called handler");
                    ((Attacking.Attack)ev.file).damage *= 3;
                })
            );
            System.Console.WriteLine("Adding modifier");
            player.StatManager.AddModifier(mod2);
            attack = (Attacking.Attack)player.StatManager.GetFile("attack");
            attack = (Attacking.Attack)player.StatManager.GetFile("attack");
            System.Console.WriteLine("Attack damage:{0}", attack.damage);
            System.Console.WriteLine("Attack pierce:{0}", attack.pierce);
            System.Console.WriteLine("Removing modifier");
            player.StatManager.RemoveChainModifier(mod2);
            attack = (Attacking.Attack)player.StatManager.GetFile("attack");
            System.Console.WriteLine("Attack damage:{0}", attack.damage);
            System.Console.WriteLine("Attack pierce:{0}", attack.pierce);

            System.Console.WriteLine("\n ------ Pools Demo ------ \n");
            var poolDef = new PoolDefinition<SubPool>();
            // other option: new PoolDefinition<EndlessSubPool>();
            PoolItem[] items = new[]
            {
                new PoolItem(0, 1),
                new PoolItem(1, 1),
                new PoolItem(2, 1),
                new PoolItem(3, 1),
                new PoolItem(4, 10)
            };
            poolDef.RegisterItems(items);

            Pool zone1dir = new Pool();

            SubPool weapons = new SubPool();
            SubPool trinkets = new SubPool();
            SubPool armor = new SubPool();

            poolDef.m_baseDir.AddDirectory("zone1", zone1dir);
            // TODO: create + add
            zone1dir.AddFile("weapons", weapons);
            zone1dir.AddFile("trinkets", trinkets);
            zone1dir.AddFile("armor", armor);

            poolDef.AddItemsToPool(items.Take(2), "zone1/weapons");
            poolDef.AddItemsToPool(items.Skip(2).Take(2), "zone1/trinkets");
            poolDef.AddItemToPool(items[4], "zone1/trinkets");

            SuperPool<SubPool> superPool = new SuperPool<SubPool>(poolDef);

            var it1 = superPool.GetNextItem("zone1/weapons");
            System.Console.WriteLine($"Item Id = {it1.id}, q = {it1.q}");
            var it2 = superPool.GetNextItem("zone1/weapons");
            System.Console.WriteLine($"Item Id = {it2.id}, q = {it2.q}");
            var it3 = superPool.GetNextItem("zone1/weapons");
            System.Console.WriteLine($"Item Id = {it3.id}, q = {it3.q}");


            System.Console.WriteLine("\n ------ TargetProvider Demo ------ \n");
            var pattern = new List<Piece>
            {
                new Piece
                {
                    pos = new IntVector2(1, 0),
                    dir = new IntVector2(1, 0),
                    reach = null
                },
                new Piece
                {
                    pos = new IntVector2(2, 0),
                    dir = new IntVector2(1, 0),
                    reach = new List<int>()
                }
            };
            var weapon = new TargetProvider<AtkTarget>
            (
                pattern: pattern,
                chain: Handlers.GeneralChain,
                stopFunc: e => !e.propagate || e.targets.Count == 0
            );
            System.Console.WriteLine($"Enemy is at {enemy.Pos}");
            var ev = new Attacking.Event { actor = player, action = playerNextAction };
            var targets = weapon.GetTargets(ev);
            foreach (var t in targets)
            {
                System.Console.WriteLine($"Entity at {t.entity.Pos} has been considered a potential target");
            }


            System.Console.WriteLine("\n ------ Inventory Demo ------ \n");
            var inventory = (Inventory)player.Inventory;

            var cyclicContainer = new CyclicItemContainer(1);
            inventory.AddContainer(0, cyclicContainer);

            var tinker = Tinker<TinkerData>.SingleHandlered<Attacking.Event>(
                Attacking.Check.ChainPath,
                e => System.Console.WriteLine("Hello from tinker applied by item")
            );
            var item = new TinkerItem(tinker, 0);

            // inventory.Equip(item) ->         // the starting point
            // item.BeEquipped(entity) ->       // it's interface method
            // entity.Tink(item.tinker) ->      // it adds data to its list
            // tinker.Tink(entity)              // creates tinker data
            inventory.Equip(item);

            world.Loop();
            System.Console.WriteLine("Looped");

            // creates excess as the size is 1
            inventory.Equip(item);
            // inventory.DropExcess() ->
            // item.BeUnequipped() ->
            // entity.Untink() +
            // world.CreateDroppedItem(id, pos)
            inventory.DropExcess();

            var entities = world.m_grid.GetCellAt(player.Pos).m_entities;
            System.Console.WriteLine($"There's {entities.Count} entities in the cell where the player is standing");

            System.Console.WriteLine("\n ------ History Demo ------ \n");
            var enemyEventsByPhases = enemy.History.Phases;
            var playerEventsByPhases = player.History.Phases;
            for (int i = 0; i < playerEventsByPhases.Length; i++)
            {
                foreach (var historyEvent in enemyEventsByPhases[i])
                {
                    System.Console.WriteLine($"Enemy did {historyEvent.eventCode.ToString()}. Position after: {historyEvent.stateAfter.pos}");
                }
                foreach (var historyEvent in playerEventsByPhases[i])
                {
                    System.Console.WriteLine($"Player did {historyEvent.eventCode.ToString()}. Position after: {historyEvent.stateAfter.pos}");
                }
            }

            System.Console.WriteLine("\n ------ Equip on Displace Demo ------ \n");
            // we don't need the entity for the next test
            enemy.Die();

            var playerMoveAction = moveAction.Copy();
            playerMoveAction.direction = IntVector2.Down;
            player.GetBehavior<Acting>().NextAction = playerMoveAction;

            var cyclicContainer2 = new CyclicItemContainer(1);
            inventory.AddContainer(1, cyclicContainer);

            var chainDefs2 = new IChainDef[0];
            var tinker2 = new Tinker<TinkerData>(chainDefs2);
            var item2 = new TinkerItem(tinker2, 1);

            var droppedItem2 = world.CreateDroppedItem(item2, player.Pos + IntVector2.Down);

            /*
            this only works because we did
            `playerFactory.AddRetoucher(Core.Retouchers.Equip.OnDisplace);`
            up top.
            */

            System.Console.WriteLine($"Player's position before moving: {player.Pos}");
            world.Loop();
            System.Console.WriteLine($"Player's new position: {player.Pos}");
            System.Console.WriteLine($"There's {world.m_grid.GetCellAt(player.Pos).m_entities.Count} entities in the cell where the player is standing");


            System.Console.WriteLine("\n ------ Tinker static reference Demo ------ \n");

            var tinker3 = TestTinkerStuff.tinker; // see the definition below
            player.TinkAndSave(tinker3);
            player.GetBehavior<Acting>().NextAction = playerMoveAction;
            world.Loop();
            player.Untink(tinker3);


            System.Console.WriteLine("\n ------ Status Demo ------ \n");

            // this has to be rethought for sure
            var status = TestStatusTinkerStuff.status;
            var flavor = new Flavor
            {
                power = 1,
                amount = 2,
                source = testStatusId
            };
            var statusedParams = new Statused.Params { flavors = new Flavor[] { flavor } };
            player.GetBehavior<Statused>().Activate(null, statusedParams);
            player.GetBehavior<Acting>().NextAction = playerMoveAction;
            world.Loop();
            player.GetBehavior<Acting>().NextAction = playerMoveAction;
            world.Loop();
            player.GetBehavior<Acting>().NextAction = playerMoveAction;
            world.Loop();

        }
    }

    public class TestTinkerData : TinkerData
    {
        public int i;
        public override void Init(Entity entity)
        {
            i = 1;
        }
    }

    public static class TestTinkerStuff
    {
        static void TestMethod1(CommonEvent commonEvent)
        {
            var data = tinker.GetStoreByEvent(commonEvent);
            System.Console.WriteLine($"Tinker says that i = {data.i}");
        }
        public static Tinker<TestTinkerData> tinker = Tinker<TestTinkerData>
            .SingleHandlered<Displaceable.Event>(Displaceable.Do.ChainPath, TestMethod1);
    }

    public static class TestStatusTinkerStuff
    {
        static void TestMethod1(CommonEvent commonEvent)
        {
            var flavor = tinker.GetStoreByEvent(commonEvent).flavor;
            System.Console.WriteLine($"Tinker says that amount = {flavor.amount}");
        }
        public static Tinker<FlavorTinkerData> tinker = Tinker<FlavorTinkerData>
            .SingleHandlered<Acting.Event>(Acting.Check.ChainPath, TestMethod1);
        public static Status<FlavorTinkerData> status =
            new Status<FlavorTinkerData>(tinker);
    }
}