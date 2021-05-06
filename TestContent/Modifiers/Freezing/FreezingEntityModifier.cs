using Hopper.Core;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Shared.Attributes;
using Hopper.Utils;

namespace Hopper.TestContent.FreezingNS
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
}