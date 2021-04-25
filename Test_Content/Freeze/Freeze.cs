using Hopper.Core;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat.Basic;
using Hopper.Shared.Attributes;
using Hopper.Utils;

namespace Hopper.TestContent.Freezing
{
    public partial class FreezingEntityModifier : IEntityModifier
    {
        public Entity outerEntity;


        [Export(Chain = "Ticking.Do", Priority = PriorityRank.High, Dynamic = true)] 
        public void RemoveByKillingOuter(FreezingEntityModifier freezing) 
        {
            // this also removes this modifier, so we're done
            freezing.outerEntity.Die();
        }

        public void Preset(Entity entity, int hp)
        {
            var transform = entity.GetTransform();
            var iceCube = World.Global.SpawnEntity(
                IceCube.Factory, transform.position, transform.orientation);
            this.outerEntity = iceCube;

            RemoveByKillingOuterHandlerWrapper.AddTo(entity);
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
            RemoveByKillingOuterHandlerWrapper.TryRemoveFrom(entity);
        }
    }

    public static partial class Freeze
    {
        public static StatusSource Source;

        public static bool TryApplyTo(Entity target, int hp)
        {
            if (!Source.CheckResistance(target, hp))
            {
                return false;
            }
            if (target.TryFreezingEntityModifier(out var modifier))
            {
                // Reset hp back up
                modifier.GetDamageable().health.amount = hp;
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

        public void RemoveFrom(Entity entity)
        {
            entity.GetFreezingEntityModifier().Unset(entity);
        }

        public bool TryRemoveFrom(Entity entity)
        {
            if (entity.TryGetFreezingEntityModifier(out var modifier))
            {
                modifier.Remove(entity);
                return true;
            }
            return false;
        }
    }
}