using System.Collections;
using System.Collections.Generic;
using Vector;
using Chains;
using Core;
using Core.Items;
using System.Linq;
using Core.Weapon;

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
            inventory.AddContainerInSlot(0, cyclicContainer);

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
            var item = new TinkerItem(tinker);

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

        }
    }


}