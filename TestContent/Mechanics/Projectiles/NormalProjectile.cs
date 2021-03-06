using Hopper.Core;
using Hopper.Core.ActingNS;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Core.Targeting;
using Hopper.Core.WorldNS;
using Hopper.Shared.Attributes;

namespace Hopper.TestContent.Projectiles
{
    [EntityType]
    public static class NormalProjectile
    {
        public static EntityFactory Factory;

        public static void AddComponents(Entity subject)
        {
            Stats.AddTo(subject, Registry.Global.Stats._map);
            Transform.AddTo(subject, Layers.PROJECTILE, 0);
            FactionComponent.AddTo(subject, Faction.Environment);
            Displaceable.AddTo(subject, 0);
            ProjectileComponent.AddTo(subject, Layers.REAL | Layers.WALL | Layers.PROJECTILE);
            Attackable.AddTo(subject, Attackness.CAN_BE_ATTACKED);
            Damageable.AddTo(subject, new Health(1));
            Ticking.AddTo(subject);
            Acting.AddTo(
                subject, 
                entity => ProjectileComponent.Action.Compile(entity.GetTransform().orientation),
                Algos.SimpleAlgo,
                Order.Projectile);
        }

        public static void InitComponents(Entity subject)
        {
            subject.GetDisplaceable().DefaultPreset();
            // subject.GetProjectileComponent()
            // subject.GetAttackable();
            subject.GetActing().DefaultPreset(subject);
            subject.GetDamageable().DefaultPreset();
            subject.GetTicking().DefaultPreset();
        }

        public static void Retouch(Entity subject) {}

    }
}