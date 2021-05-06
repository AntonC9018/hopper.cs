using Hopper.Core;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Shared.Attributes;
using Hopper.TestContent.Stat;
using Hopper.Utils;

namespace Hopper.TestContent.Freezing
{
    public partial class FreezingEntityModifier : IComponent
    {
        public Entity outerEntity;


        [Export(Chain = "Ticking.Do", Priority = PriorityRank.High, Dynamic = true)] 
        public void TickOuterHealth() 
        {
            if (--outerEntity.GetDamageable().health.amount <= 0)
            {
                // this also removes this modifier, so we're done
                outerEntity.Die();
            }
        }

        public void Preset(Entity entity, int hp)
        {
            var transform = entity.GetTransform();
            var iceCube = World.Global.SpawnEntity(
                IceCube.Factory, transform.position, transform.orientation);
            this.outerEntity = iceCube;

            TickOuterHealthHandlerWrapper.HookTo(entity);
        }

        public void Unset(Entity entity)
        {
            if (!outerEntity.IsDead())
            {
                outerEntity.Die();
            }
            Assert.That(!entity.HasFreezingEntityModifier());
        }

        public void RemoveLogic(Entity entity)
        {
            entity.GetTransform().ResetInGrid();
            TickOuterHealthHandlerWrapper.UnhookFrom(entity);
        }
    }

    public static class Freeze
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