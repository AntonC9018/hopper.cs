using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Registries;
using Hopper.Core.Stats;
using Hopper.Core.Stats.Basic;
using Hopper.Utils.Vector;

namespace Hopper.Test_Content.Floor
{
    public static class Spikes
    {
        public static EntityFactory<Trap> Factory = CreateFactory();
        public static readonly DirectedAction AttackAction = Action.CreateBehavioral<Attacking>();
        public static Attack AttackStat =>
            new Attack
            {
                sourceId = Attack.BasicSource.Id,
                power = 2,
                pierce = 1,
                damage = 3
            };

        private static DefaultStats GetDefaultStats(PatchArea patchArea)
        {
            return new DefaultStats(patchArea).Set(Attack.Path, AttackStat);
        }

        public static EntityFactory<Trap> CreateFactory()
        {
            return new EntityFactory<Trap>()
                .AddBehavior(Attackable.DefaultPreset)
                .AddBehavior(Attacking.Preset)
                .AddBehavior(Acting.Preset(
                    new Acting.Config(
                        Algos.SimpleAlgo,
                        e => AttackAction.ToDirectedParticular(IntVector2.Zero)
                    )
                ))
                .SetDefaultStats(GetDefaultStats);
        }
    }
}