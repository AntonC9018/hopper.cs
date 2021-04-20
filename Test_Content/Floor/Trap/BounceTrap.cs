using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Registries;
using Hopper.Core.Stat;
using Hopper.Core.Stat.Basic;

namespace Hopper.Test_Content.Floor
{
    public static class BounceTrap
    {
        public static EntityFactory<Trap> Factory = CreateFactory();
        public static readonly DirectedAction BounceAction = Action.CreateBehavioral<Bouncing>();
        public static readonly Push PushStat =
            new Push
            {
                sourceId = Bounce.Source.Id,
                power = 2,
                pierce = 1,
                distance = 1
            };

        private static DefaultStats GetDefaultStats(PatchArea patchArea)
        {
            return new DefaultStats(patchArea).Set(Push.Path, PushStat);
        }

        public static EntityFactory<Trap> CreateFactory()
        {
            return new EntityFactory<Trap>()
                .AddBehavior(Attackable.DefaultPreset)
                .AddBehavior(Acting.Preset(
                    new Acting.Config(
                        Algos.SimpleAlgo,
                        e => BounceAction.ToDirectedParticular(e.Orientation)
                    )
                ))
                .AddBehavior(Bouncing.Preset)
                .SetDefaultStats(GetDefaultStats);
            // .SetDefaultStats(defaultStats); // run at patching
        }
    }
}