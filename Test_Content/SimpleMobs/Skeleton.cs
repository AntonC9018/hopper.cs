using Hopper.Core;
using Hopper.Core.Behaviors.Basic;
using Hopper.Core.Retouchers;

namespace Hopper.Test_Content.SimpleMobs
{
    public static class Skeleton
    {
        public static readonly EntityFactory<Entity> Factory = CreateFactory();
        public static EntityFactory<Entity> CreateFactory()
        {
            return new EntityFactory<Entity>()
                .AddBehavior<Acting>(new Acting.Config(Algos.EnemyAlgo))
                .AddBehavior<Sequential>(new Sequential.Config(CreateSequenceData()))

                .AddBehavior<Attacking>()
                .AddBehavior<Moving>()

                .AddBehavior<Displaceable>()
                .AddBehavior<Attackable>()
                .AddBehavior<Pushable>()
                .AddBehavior<Statused>()

                .Retouch(Skip.NoPlayer)
                .Retouch(Skip.BlockedMove)
                .Retouch(Reorient.OnActionSuccess);
        }

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