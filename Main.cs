using System.Collections.Generic;
using Vector;
using Chains;
using Core;
using Core.Items;
using System.Linq;
using Core.Weapon;
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

            var playerFactory = new EntityFactory<Entity>();
            playerFactory.AddBehavior(Attackable.s_factory);
            playerFactory.AddBehavior(Attacking.s_factory);
            playerFactory.AddBehavior(Displaceable.s_factory);
            playerFactory.AddBehavior(Moving.s_factory);
            playerFactory.AddBehavior(Pushable.s_factory);
            System.Console.WriteLine("Set up playerFactory");

            Acting.Config playerActingConf = new Acting.Config
            {
                doAction = Algos.SimpleAlgo
            };

            playerFactory.AddBehavior(Acting.s_factory, playerActingConf);
            playerFactory.AddRetoucher(Core.Retouchers.Skip.EmptyAttack);

            // this one's for the equip demo
            playerFactory.AddRetoucher(Core.Retouchers.Equip.OnDisplace);


            var enemyFactory = new EntityFactory<Entity>();
            enemyFactory.AddBehavior(Attackable.s_factory);
            enemyFactory.AddBehavior(Attacking.s_factory);
            enemyFactory.AddBehavior(Displaceable.s_factory);
            enemyFactory.AddBehavior(Moving.s_factory);
            enemyFactory.AddBehavior(Pushable.s_factory);


            Acting.Config enemyActingConf = new Acting.Config
            {
                doAction = Algos.EnemyAlgo
            };

            enemyFactory.AddBehavior(Acting.s_factory, enemyActingConf);


            SimpleAction attackAction = new SimpleAction(Attacking.s_factory.id);
            SimpleAction moveAction = new SimpleAction(Moving.s_factory.id);
            CompositeAction attackMoveAction = new CompositeAction(
                new SimpleAction[] { attackAction, moveAction }
            );

            StepData[] stepData =
            {
                new StepData { action = null },
                new StepData { action = attackMoveAction, movs = Movs.Basic }
            };

            var sequenceConfig = new Sequential.Config(stepData);

            enemyFactory.AddBehavior(Sequential.s_factory, sequenceConfig);
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
            player.beh_Acting.m_nextAction = playerNextAction;
            System.Console.WriteLine("Set player action");
            System.Console.WriteLine("\n ------ Modifier Demo ------ \n");

            var mod = new StatModifier("attack", new Attacking.Attack { damage = 1 });
            var attack = (Attacking.Attack)player.m_statManager.GetFile("attack");
            System.Console.WriteLine("Attack damage:{0}", attack.damage);
            System.Console.WriteLine("Attack pierce:{0}", attack.pierce);
            System.Console.WriteLine("Adding modifier");
            player.m_statManager.AddModifier(mod);
            // attack = (Attacking.Attack)player.m_statManager.GetFile("attack");
            System.Console.WriteLine("Attack damage:{0}", attack.damage);
            System.Console.WriteLine("Attack pierce:{0}", attack.pierce);
            System.Console.WriteLine("Removing modifier");
            player.m_statManager.RemoveModifier(mod);
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
            player.m_statManager.AddModifier(mod2);
            attack = (Attacking.Attack)player.m_statManager.GetFile("attack");
            attack = (Attacking.Attack)player.m_statManager.GetFile("attack");
            System.Console.WriteLine("Attack damage:{0}", attack.damage);
            System.Console.WriteLine("Attack pierce:{0}", attack.pierce);
            System.Console.WriteLine("Removing modifier");
            player.m_statManager.RemoveChainModifier(mod2);
            attack = (Attacking.Attack)player.m_statManager.GetFile("attack");
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
            var ev = new Attacking.Event { actor = player, action = playerNextAction };
            var targets = weapon.GetTargets(ev);
            foreach (var t in targets)
            {
                System.Console.WriteLine($"Entity at {t.entity.m_pos} has been considered a potential target");
            }


            System.Console.WriteLine("\n ------ Inventory Demo ------ \n");
            var inventory = new Inventory(player);

            var cyclicContainer = new CyclicItemContainer(1);
            inventory.AddContainer(0, cyclicContainer);

            var chainDefs = new IChainDef[]
            {
                new ChainDef<Attacking.Event>
                (
                    "attack:check",
                    new EvHandler<Attacking.Event>(
                        e => System.Console.WriteLine("Hello from tinker applied by item")
                    )
                )
            };
            var tinker = new Tinker(chainDefs);
            var item = new TinkerItem(tinker, 0);

            // inventory.Equip(item) ->         // the starting point
            // item.BeEquipped(entity) ->       // it's interface method
            // entity.Tink(item.tinker) ->      // it adds data to its list
            // tinker.Tink(entity)              // creates tinker data
            inventory.Equip(item);

            world.m_state.Loop();
            System.Console.WriteLine("Looped");

            // creates excess as the size is 1
            inventory.Equip(item);
            // inventory.DropExcess() ->
            // item.BeUnequipped() ->
            // entity.Untink() +
            // world.CreateDroppedItem(id, pos)
            inventory.DropExcess();

            var entities = world.m_grid.GetCellAt(player.m_pos).m_entities;
            System.Console.WriteLine($"There's {entities.Count} entities in the cell where the player is standing");

            System.Console.WriteLine("\n ------ History Demo ------ \n");
            var enemyEventsByPhases = enemy.m_history.Phases;
            var playerEventsByPhases = player.m_history.Phases;
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
            enemy.b_isDead = true;
            enemy.RemoveFromGrid();

            var playerMoveAction = moveAction.Copy();
            playerMoveAction.direction = IntVector2.Down;
            player.beh_Acting.m_nextAction = playerMoveAction;
            player.Inventory = inventory;

            var cyclicContainer2 = new CyclicItemContainer(1);
            inventory.AddContainer(1, cyclicContainer);

            var chainDefs2 = new IChainDef[0];
            var tinker2 = new Tinker(chainDefs);
            var item2 = new TinkerItem(tinker2, 1);

            var droppedItem2 = world.CreateDroppedItem(item2, player.m_pos + IntVector2.Down);

            /*
            this only works because we did
            `playerFactory.AddRetoucher(Core.Retouchers.Equip.OnDisplace);`
            up top.
            */

            System.Console.WriteLine($"Player's position before moving: {player.m_pos}");
            world.m_state.Loop();
            System.Console.WriteLine($"Player's new position: {player.m_pos}");
            System.Console.WriteLine($"There's {world.m_grid.GetCellAt(player.m_pos).m_entities.Count} entities in the cell where the player is standing");
        }
    }


}