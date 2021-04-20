using System.Runtime.Serialization;
using Hopper.Core.Components.Basic;
using Hopper.Core.Items;
using Hopper.Core.Retouchers;
using Hopper.Core.Stat.Basic;
using Hopper.Core.Targeting;
using Hopper.Core.History;
using Hopper.Core.Stat;
using Hopper.Shared.Attributes;

namespace Hopper.Core
{
    [EntityType]
    public static class Player
    {
        /* TODO: generate such code from json
            {
                name: "Player",
                components: [
                    
                    { name = "Acting"
                    , "_DoAction" = "SimpleAlgo", // the default should be this?
                    , "_CalculateAction" = null   // should not be required
                    },

                    { name = "

                    }
                ]
            }
        */

        public static EntityFactory Factory;

        public static void AddComponents(Entity subject)
        {
            // Apply these one after the other.
            // This will only initialize the inject variables of the behavior / component.
            // So, apply will be autogenerated for the different behaviors based on their injects.
            // Or do it even smarter? 
            // So, since I'd like to make components structs in the future and store them somewhere
            // central (optionally), these can actually reference a global storage for them.
            
            // So this just adds the behavior
            Acting  .AddTo(subject, null, null, Order.Player);
            Moving  .AddTo(subject);
            Digging .AddTo(subject);
            Pushable.AddTo(subject);
            Statused.AddTo(subject);
            Attacking   .AddTo(subject);
            Attackable  .AddTo(subject, Attackness.ALWAYS);
            Damageable  .AddTo(subject, new Health(5));
            Displaceable.AddTo(subject, ExtendedLayer.BLOCK);
            Ticking.AddTo(subject);

            Faction.AddTo(subject, Faction.Flags.Player);
            Transform.AddTo(subject, Layer.REAL);
            Inventory.AddTo(subject);

            // TODO: pass this an action
            Controllable.AddTo(subject, null);

            // TODO: rename the namespaces
            Stats.AddTo(subject);
            History.History.AddTo(subject);
        }

        public static void InitComponents(Entity subject)
        {
            subject.GetActing() .DefaultPreset(subject);
            subject.GetMoving() .DefaultPreset();
            subject.GetDigging().DefaultPreset();
            subject.GetTicking().DefaultPreset();
            subject.GetPushable()  .DefaultPreset();
            subject.GetStatused()  .DefaultPreset();
            subject.GetAttacking() .DefaultPreset();
            subject.GetAttackable().DefaultPreset();
            subject.GetDamageable().DefaultPreset();
            subject.GetDisplaceable().DefaultPreset();
            // subject.TryGetFactionComponent()?.
            // subject.TryGetTransform()?.
            // subject.TryGetInventory()?.
            // subject.TryGetControllable()?.
            // subject.TryGetStats()?.
            // subject.TryGetHistory()?.
        }

        public static void Retouch(Entity subject)
        {
            Skip.SkipEmptyAttackHandlerWrapper.AddTo(subject);
            Skip.SkipEmptyAttackHandlerWrapper.AddTo(subject);
            Equip.OnDisplaceHandlerWrapper.AddTo(subject);
            Reorient.OnActionSuccessHandlerWrapper.AddTo(subject);
        }
    }
}