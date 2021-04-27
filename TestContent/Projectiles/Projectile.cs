using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Core.Stat;
using Hopper.Core.Targeting;
using Hopper.Shared.Attributes;

namespace Hopper.TestContent.Projectiles
{
    [EntityType]
    public static class NormalProjectile
    {
        public static EntityFactory Factory;
        
        public static DirectedAction ProjectileAction =>
            Action.CreateFromActivateable(ProjectileComponent.Index);

        public static void AddComponent(Entity subject)
        {
            Stats.AddTo(subject, Registry.Global._defaultStats);
            Transform.AddTo(subject, Layer.PROJECTILE);
            Faction.AddTo(subject, Faction.Flags.Environment);
            Displaceable.AddTo(subject, 0);
            ProjectileComponent.AddTo(subject, Layer.REAL | Layer.WALL | Layer.PROJECTILE);
            Attackable.AddTo(subject, Attackness.CAN_BE_ATTACKED);
            Damageable.AddTo(subject, new Health(1));
            Acting.AddTo(
                subject, 
                entity => ProjectileAction.ToDirectedParticular(entity.GetTransform().orientation),
                Algos.SimpleAlgo,
                Order.Projectile);
        }

        public static void InitComponents(Entity subject)
        {
            subject.GetDisplaceable().DefaultPreset();
            // subject.GetProjectileComponent()
            subject.GetAttackable().DefaultPreset();
            subject.GetActing().DefaultPreset(subject);
            subject.GetDamageable().DefaultPreset();
        }

        public static void Retouch(Entity subject) {}

    }
}