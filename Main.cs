using System.Collections;
using System.Collections.Generic;
using Vector;
using Chains;
using Core;
using Core.Items;
using System.Linq;

// Hello World! program
namespace Hopper
{
    class Hello
    {

        static void Main(string[] args)
        {
            System.Console.WriteLine("Hello");
            World world = new World
            {
                m_grid = new GridManager(5, 5),
                m_state = new WorldStateManager()
            };
            System.Console.WriteLine("Created world");

            var playerFactory = new EntityFactory(typeof(Entity));
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


            var enemyFactory = new EntityFactory(typeof(Entity));
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

            enemy.Init(new Vector2(1, 1), world);
            world.m_state.AddEntity(enemy);
            world.m_grid.Reset(enemy);
            System.Console.WriteLine("Enemy set in world");

            player.Init(new Vector2(1, 2), world);
            world.m_state.AddPlayer(player);
            world.m_grid.Reset(player);
            System.Console.WriteLine("Player set in world");

            var playerNextAction = attackMoveAction.Copy();
            playerNextAction.direction = new Vector2(0, 1);
            player.beh_Acting.m_nextAction = playerNextAction;
            System.Console.WriteLine("Set player action");

            world.m_state.Loop();
            System.Console.WriteLine("Looped");
            System.Console.WriteLine($"Player's new position {player.m_pos}");

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


            var poolDef = new PoolDefinition<SubPool>();
            // other option: new PoolDefinition<EndlessSubPool>();
            Item[] items = new[]
            {
                new Item(0, 1),
                new Item(1, 1),
                new Item(2, 1),
                new Item(3, 1),
                new Item(4, 10)
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


            // world.m_state.Loop();
            // System.Console.WriteLine("Looped");
            // System.Console.WriteLine($"Player's new position {player.m_pos}");

        }
    }


}