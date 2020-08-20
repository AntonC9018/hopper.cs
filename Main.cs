using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Chains;
using Core;

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

            player.Init(new Vector2(2, 2), world);
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

            // world.m_state.Loop();
            // System.Console.WriteLine("Looped");
            // System.Console.WriteLine($"Player's new position {player.m_pos}");

        }
    }


}