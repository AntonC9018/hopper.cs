using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.TestContent.Stat;

namespace Hopper.TestContent.FreezingNS
{
    public static class Freezing
    {
        public static bool TryApplyTo(Entity target, int power, int hp)
        {
            if (target.CanNotResist(Stat.Freeze.Source, power))
            {
                return false;
            }
            if (target.TryGetFreezingEntityModifier(out var modifier))
            {
                // Reset hp back up
                modifier.outerEntity.GetDamageable().health.amount = hp;
                // damageable.health.amount += hp;

                // These two are possible for selection?
                // freezing.amount += amount;
                // freezing.amount = amount;
            }
            else
            {   
                ApplyTo(target, hp);
            }
            return true;
        }

        public static void ApplyTo(Entity target, int hp)
        {
            var modifier = new FreezingEntityModifier();
            modifier.Preset(target, hp);
        }

        public static void RemoveFrom(Entity entity)
        {
            entity.GetFreezingEntityModifier().Unset(entity);
        }

        public static bool TryRemoveFrom(Entity entity)
        {
            if (entity.TryGetFreezingEntityModifier(out var modifier))
            {
                modifier.Unset(entity);
                return true;
            }
            return false;
        }
    }
}