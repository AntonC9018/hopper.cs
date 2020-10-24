using Core;
using Core.Behaviors;
using Core.Stats;
using Core.Stats.Basic;

namespace Test
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

        public static StatManager defaultStats = GetDefaultStats();
        private static StatManager GetDefaultStats()
        {
            var stats = new StatManager();
            stats.GetRaw(Push.Path.String, PushStat);
            return stats;
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
                .AddSetupListener(e => e.Stats.DefaultStats = defaultStats);
        }
    }
}