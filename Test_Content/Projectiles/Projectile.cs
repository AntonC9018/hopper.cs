using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Registries;
using Hopper.Core.Stat;
using Hopper.Core.Stat.Basic;

namespace Hopper.Test_Content.Projectiles
{
    public class Projectile : Entity
    {
        public override Layer Layer => Layer.PROJECTILE;

        public static DefaultStats CreateDefaultStats(PatchArea patchArea)
        {
            return new DefaultStats(patchArea).Set(Health.Path, new Health { amount = 1 });
        }

        public static DirectedAction ProjectileAction =>
            Action.CreateBehavioral<ProjectileBehavior>();

        public static EntityFactory<Projectile> CreateFactory()
        {
            return new EntityFactory<Projectile>()
                .AddBehavior(Displaceable.Preset(0)) // not blocked by anything
                .AddBehavior(Attackable.DefaultPreset)
                .AddBehavior(Damageable.Preset)
                .SetDefaultStats(CreateDefaultStats)
                .AddBehavior(ProjectileBehavior.Preset(Layer.REAL | Layer.WALL | Layer.PROJECTILE))
                .AddBehavior(Acting.Preset(
                    new Acting.Config(Algos.SimpleAlgo, e => ProjectileAction.ToDirectedParticular(e.Orientation)))
                );

        }

        public static EntityFactory<Projectile> SimpleFactory = CreateFactory();
    }
}