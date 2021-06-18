using Hopper.Core.ActingNS;
using Hopper.Core.Components.Basic;
using Hopper.Core.Items;
using Hopper.Core.Retouchers;
using Hopper.Core.Stat;
using Hopper.Core.Targeting;
using Hopper.Core.WorldNS;
using Hopper.Shared.Attributes;

namespace Hopper.Core
{
    [EntityType]
    public static class Player
    {
        public static EntityFactory Factory;

        public static void AddComponents(EntityFactory factory)
        {
            // Apply these one after the other.
            // This will only initialize the inject variables of the behavior / component.
            // So, apply will be autogenerated for the different behaviors based on their injects.
            // Or do it even smarter? 
            // So, since I'd like to make components structs in the future and store them somewhere
            // central (optionally), these can actually reference a global storage for them.
            
            // So this just adds the behavior
            Acting  .AddTo(factory, null, Algos.SimpleAlgo, Order.Player);
            Moving  .AddTo(factory);
            Digging .AddTo(factory);
            Pushable.AddTo(factory);
            Attacking   .AddTo(factory, Attacking.GetTargetProviderFromInventory, Layers.REAL, Faction.Enemy|Faction.Environment);
            Attackable  .AddTo(factory, Attackness.ALWAYS);
            Damageable  .AddTo(factory, new Health(5));
            Displaceable.AddTo(factory, Layers.BLOCK);
            Ticking.AddTo(factory);

            FactionComponent.AddTo(factory, Faction.Player);
            Transform.AddTo(factory, Layers.REAL, TransformFlags.Default);
            Inventory.AddTo(factory);
            Inventory.AddInitTo(factory);

            // TODO: pass this an action
            Controllable.AddTo(factory, 
                // The default action is the AttackDigMove action.
                Action.Compose(Attacking.Action, Digging.Action, Moving.Action));

            // TODO: rename the namespaces
            Stats.AddTo(factory, Registry.Global.Stats._map);
        }

        public static void InitComponents(Entity subject)
        {
            subject.GetActing() .DefaultPreset(subject);
            subject.GetMoving() .DefaultPreset();
            // subject.GetDigging().DefaultPreset();
            subject.GetTicking().DefaultPreset();
            subject.GetPushable()  .DefaultPreset();
            subject.GetAttacking() .SkipEmptyAttackPreset();
            subject.GetAttackable();
            subject.GetDamageable().DefaultPreset();
            subject.GetDisplaceable().DefaultPreset();
            // subject.TryGetFactionComponent()?.
            // subject.TryGetTransform()?.
            // subject.TryGetInventory()?.
            // subject.TryGetControllable()?.
            // subject.TryGetStats()?.
            // subject.TryGetHistory()?.
        }

        public static void Retouch(EntityFactory factory)
        {
            Skip.SkipEmptyAttackHandlerWrapper.HookTo(factory);
            Equip.OnDisplaceHandlerWrapper.HookTo(factory);
            Reorient.OnActionSuccessHandlerWrapper.HookTo(factory);
            Skip.SkipEmptyDigHandlerWrapper.HookTo(factory);
        }
    }
}