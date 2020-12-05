using Hopper.Core;
using Hopper.Core.Behaviors;

namespace Hopper.Test_Content
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

                .Retouch(Hopper.Core.Retouchers.Skip.NoPlayer)
                .Retouch(Hopper.Core.Retouchers.Skip.BlockedMove)
                .Retouch(Hopper.Core.Retouchers.Reorient.OnActionSuccess);
        }
    }
}