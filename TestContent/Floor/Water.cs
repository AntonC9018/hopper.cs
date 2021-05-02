using Hopper.Core;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Shared.Attributes;
using Hopper.TestContent;

namespace Hopper.TestContent.Floor
{
    // TODO: implement fully
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

        [Export(Chain = "Attacking.Do", Priority = PriorityRank.High, Dynamic = true)]
        public bool PreventActionAndDecreaseAmount_Attacking(Entity actor) 
            => PreventActionAndDecreaseAmount(actor);

        [Export(Chain = "Digging.Do", Priority = PriorityRank.High, Dynamic = true)]
        public bool PreventActionAndDecreaseAmount_Digging(Entity actor) 
            => PreventActionAndDecreaseAmount(actor);

        [Export(Chain = "Displaceable.Do", Priority = PriorityRank.High, Dynamic = true)]
        public bool PreventActionAndDecreaseAmount_Displaceable(Entity actor) 
            => PreventActionAndDecreaseAmount(actor);

        
        // public static void RemoveOnLeave(Transform subject)
        // {
        //     if (subject.entity.TryGetStuckInWaterEntityModifier(out var modifier))
        //     {

        //     }
        // } 

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

    // [EntityType]
    public static class Water
    {
        public static EntityFactory Factory;

        public static void AddComponents(Entity subject)
        {
            Stats.AddTo(subject, Registry.Global._defaultStats);
            Transform.AddTo(subject, Layer.REAL);
            FactionComponent.AddTo(subject, Faction.Environment);

        }

        public static void InitComponents(Entity subject)
        {
        }

        public static void Retouch(Entity subject)
        {
        }
    }
}