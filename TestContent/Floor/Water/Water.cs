using Hopper.Core;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Shared.Attributes;
using Hopper.TestContent;

namespace Hopper.TestContent.Floor
{
    // Another way of implementing this is substituting another action for the vector action
    // The way I'm doing it here is too specific and also involves boilerplate 
    public partial class StuckInWaterEntityModifier : IComponent
    {
        [Inject] public int amount;

        public bool PreventActionAndDecreaseAmount(Entity actor)
        {
            if (amount-- == 0)
            {
                Remove(actor);
            }
            return false;
        }

        [Export(Chain = "Attacking.Do", Dynamic = true)]
        public bool PreventActionAndDecreaseAmount_Attacking(Entity actor) 
            => PreventActionAndDecreaseAmount(actor);

        [Export(Chain = "Digging.Do", Dynamic = true)]
        public bool PreventActionAndDecreaseAmount_Digging(Entity actor) 
            => PreventActionAndDecreaseAmount(actor);

        [Export(Chain = "Displaceable.Do", Dynamic = true)]
        public bool PreventActionAndDecreaseAmount_Displaceable(Entity actor) 
            => PreventActionAndDecreaseAmount(actor);

        public void Preset(Entity subject)
        {
            PreventActionAndDecreaseAmount_AttackingHandlerWrapper.TryHookTo(subject);
            PreventActionAndDecreaseAmount_DiggingHandlerWrapper.TryHookTo(subject);
            PreventActionAndDecreaseAmount_DisplaceableHandlerWrapper.TryHookTo(subject);
        }

        public void Unset(Entity entity)
        {
            PreventActionAndDecreaseAmount_AttackingHandlerWrapper.UnhookFrom(entity);
            PreventActionAndDecreaseAmount_DiggingHandlerWrapper.UnhookFrom(entity);
            PreventActionAndDecreaseAmount_DisplaceableHandlerWrapper.UnhookFrom(entity);
        }

        public void Remove(Entity entity)
        {
            Unset(entity);
            entity.RemoveComponent(Index);
        }
    }

    public static class StuckInWater
    {
        public static void ApplyTo(Entity entity)
        {
        }
    }

    [EntityType]
    public static class Water
    {
        public static EntityFactory Factory;

        public static void AddComponents(Entity subject)
        {
            Stats.AddTo(subject, Registry.Global._defaultStats);
            Transform.AddTo(subject, Layer.REAL);
            Faction.AddTo(subject, Faction.Flags.Environment);
            
        }

        public static void InitComponents(Entity subject)
        {
        }

        public static void Retouch(Entity subject)
        {
        }
    }

        private void ListenCell()
        {
            this.GetCell().EnterEvent += ApplyStuck;
            this.GetCell().LeaveEvent += RemoveStuck;
            DieEvent += DieHandler;
        }

        private void ApplyStuck(Entity entity)
        {
            if (entity.Layer.IsOfLayer(m_targetedLayer))
            {
                Stuck.Status.TryApplyAuto(this, entity);
            }
        }

        private void RemoveStuck(Entity entity)
        {
            var status = Stuck.Status;
            if (status.IsApplied(entity))
            {
                status.m_tinker.GetStore(entity).amount = 0;
            }
        }
    }
}