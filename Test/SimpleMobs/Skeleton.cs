using Core;
using Core.Behaviors;

namespace Test
{
    public static class Skeleton
    {
        public static EntityFactory<Entity> Factory;

        static Step[] CreateSequenceData()
        {
            var moveAction = new BehaviorAction<Moving>();
            var attackAction = new BehaviorAction<Attacking>();
            var attackMoveAction = new CompositeAction(
                new Action[] { }
            );

            var stepData = new Step[]
            {
                new Step
                {
                    action = attackMoveAction,
                    movs = Movs.Basic
                },
                new Step()
            };

            return stepData;
        }

        static Skeleton()
        {
            Factory = new EntityFactory<Entity>()
                .AddBehavior<Acting>(new Acting.Config { DoAction = Algos.EnemyAlgo })
                .AddBehavior<Sequential>(new Sequential.Config(CreateSequenceData()))

                .AddBehavior<Attacking>()
                .AddBehavior<Moving>()

                .AddBehavior<Displaceable>()
                .AddBehavior<Attackable>()
                .AddBehavior<Pushable>()
                .AddBehavior<Statused>();
        }
    }
}