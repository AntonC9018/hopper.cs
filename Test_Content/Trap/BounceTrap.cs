using Hopper.Core;
using Hopper.Core.Behaviors.Basic;
using Hopper.Core.Stats;
using Hopper.Core.Stats.Basic;

namespace Hopper.Test_Content.Trap
{
    public class BounceTrap : Entity
    {
        public override Layer Layer => Layer.TRAP;
        public static Action action = new BehaviorAction<Bouncing>();

        public static Push PushStat(Registry registry) =>
            new Push
            {
                sourceId = Bounce.Source.GetId(registry),
                power = 2,
                pierce = 1,
                distance = 1
            };

        private static DefaultStats GetDefaultStats(Registry registry)
        {
            return new DefaultStats(registry).Set(Push.Path, PushStat(registry));
        }

        public static EntityFactory<BounceTrap> CreateFactory()
        {
            return new EntityFactory<BounceTrap>()
                .AddBehavior<Attackable>()
                .AddBehavior<Acting>(new Acting.Config(Algos.SimpleAlgo,
                        e => action.Copy().WithDir(e.Orientation))
                )
                .AddBehavior<Bouncing>()
                .SetDefaultStats(GetDefaultStats);
            // .SetDefaultStats(defaultStats); // run at patching
        }
    }
}