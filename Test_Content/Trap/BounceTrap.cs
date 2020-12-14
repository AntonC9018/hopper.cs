using Hopper.Core;
using Hopper.Core.Behaviors.Basic;
using Hopper.Core.Stats;
using Hopper.Core.Stats.Basic;

namespace Hopper.Test_Content.Trap
{
    public class BounceTrap : Entity
    {
        public static readonly EntityFactory<BounceTrap> Factory = CreateFactory();

        public override Layer Layer => Layer.TRAP;
        public static readonly Action action = new BehaviorAction<Bouncing>();

        public static readonly Push PushStat =
            new Push
            {
                sourceId = Bounce.Source.Id,
                power = 2,
                pierce = 1,
                distance = 1
            };

        private static DefaultStats GetDefaultStats(Repository repository)
        {
            return new DefaultStats(repository).Set(Push.Path, PushStat);
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