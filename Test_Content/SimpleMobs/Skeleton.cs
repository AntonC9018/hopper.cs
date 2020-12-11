using Hopper.Core;
using Hopper.Core.Behaviors;
using Hopper.Core.Retouchers;

namespace Hopper.Test_Content
{
    public static class Skeleton
    {
        public static EntityFactory<Entity> Factory(CoreRetouchers retocuhers) =>
            new EntityFactory<Entity>()
                .AddBehavior<Acting>(new Acting.Config(Algos.EnemyAlgo))
                .AddBehavior<Sequential>(new Sequential.Config(CreateSequenceData()))

                .AddBehavior<Attacking>()
                .AddBehavior<Moving>()

                .AddBehavior<Displaceable>()
                .AddBehavior<Attackable>()
                .AddBehavior<Pushable>()
                .AddBehavior<Statused>()

                .Retouch(retocuhers.Skip.NoPlayer)
                .Retouch(retocuhers.Skip.BlockedMove)
                .Retouch(retocuhers.Reorient.OnActionSuccess);

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
    }
}