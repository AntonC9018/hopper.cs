using Hopper.Core;
using Hopper.Core.Behaviors.Basic;
using Hopper.Core.Registries;
using Hopper.Core.Stats;
using Hopper.Core.Stats.Basic;

namespace Hopper.Test_Content.Projectiles
{
    public class Projectile : Entity
    {
        public override Layer Layer => Layer.PROJECTILE;

        public static DefaultStats CreateDefaultStats(PatchArea patchArea)
        {
            return new DefaultStats(patchArea).Set(Health.Path, new Health { amount = 1 });
        }

        public static BehaviorAction<ProjectileBehavior> ProjectileAction =>
            new BehaviorAction<ProjectileBehavior>();

        public static EntityFactory<Projectile> CreateFactory()
        {
            return new EntityFactory<Projectile>()
                .AddBehavior(Displaceable.Preset(0)) // not blocked by anything
                .AddBehavior(Attackable.DefaultPreset)
                .AddBehavior(Damageable.Preset)
                .SetDefaultStats(CreateDefaultStats)
                .AddBehavior(ProjectileBehavior.Preset(Layer.REAL | Layer.WALL | Layer.PROJECTILE))
                .AddBehavior(Acting.Preset(
                    new Acting.Config(Algos.SimpleAlgo, e => ProjectileAction.WithDir(e.Orientation)))
                );

        }

        public static readonly EntityFactory<Projectile> SimpleFactory = CreateFactory();
    }
}