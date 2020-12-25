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
                .AddBehavior(Acting.Preset(new Acting.Config(Algos.EnemyAlgo)))
                .AddBehavior(Sequential.Preset(new Sequential.Config(CreateSequenceData())))

                .AddBehavior(Attacking.Preset)
                .AddBehavior(Moving.Preset)

                .AddBehavior(Displaceable.DefaultPreset)
                .AddBehavior(Attackable.DefaultPreset)
                .AddBehavior(Pushable.Preset)
                .AddBehavior(Statused.Preset)

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