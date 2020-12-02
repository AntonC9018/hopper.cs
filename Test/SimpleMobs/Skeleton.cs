using Core;
using Core.Behaviors;

namespace Test
{
    public static class Skeleton
    {
        public static EntityFactory<Entity> Factory;

        static Step[] CreateSequenceData()
        {
            var attackAction = new BehaviorAction<Attacking>();
            var moveAction = new BehaviorAction<Moving>();
            var attackMoveAction = new CompositeAction(
                new Action[] { attackAction, moveAction }
            );

            var stepData = new Step[]
            {
                new Step
                {
                    action = attackMoveAction,
                    movs = Movs.Basic
                },
                new Step()
                {
                }
            };

            return stepData;
        }

        static Skeleton()
        {
            Factory = new EntityFactory<Entity>()
                .AddBehavior<Acting>(new Acting.Config(Algos.EnemyAlgo))
                .AddBehavior<Sequential>(new Sequential.Config(CreateSequenceData()))

                .AddBehavior<Attacking>()
                .AddBehavior<Moving>()

                .AddBehavior<Displaceable>()
                .AddBehavior<Attackable>()
                .AddBehavior<Pushable>()
                .AddBehavior<Statused>()

                .Retouch(Core.Retouchers.Skip.NoPlayer)
                .Retouch(Core.Retouchers.Skip.BlockedMove)
                .Retouch(Core.Retouchers.Reorient.OnActionSuccess);
        }
    }
}