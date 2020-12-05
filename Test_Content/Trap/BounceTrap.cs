using Hopper.Core;
using Hopper.Core.Behaviors;
using Hopper.Core.Stats;
using Hopper.Core.Stats.Basic;

namespace Hopper.Test_Content
{
    public class BounceTrap : Entity
    {
        public override Layer Layer => Layer.TRAP;
        public static Action action = new BehaviorAction<Bouncing>();

        public static Push.Source BounceSource = new Push.Source();
        public static Push PushStat = new Push
        {
            sourceId = BounceSource.Id,
            power = 2,
            pierce = 1,
            distance = 1
        };

        public static DefaultStats defaultStats = GetDefaultStats();
        private static DefaultStats GetDefaultStats()
        {
            return new DefaultStats().Set(Push.Path, PushStat);
        }

        public static EntityFactory<BounceTrap> Factory = CreateFactory();
        public static EntityFactory<BounceTrap> CreateFactory()
        {
            return new EntityFactory<BounceTrap>()
                .AddBehavior<Attackable>()
                .AddBehavior<Acting>(new Acting.Config(Algos.SimpleAlgo,
                        e => action.Copy().WithDir(e.Orientation))
                )
                .AddBehavior<Bouncing>()
                .SetDefaultStats(defaultStats);
        }
    }
}