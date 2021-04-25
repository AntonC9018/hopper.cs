using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Items;
using Hopper.Utils.Vector;
using Hopper.Utils.Chains;
using Hopper.Shared.Attributes;
using Hopper.Core.Components;

namespace Hopper.TestContent
{
    [EntityType(Abstract = true)]
    public static class Shield
    {
        public static EntityFactory Factory;
        [Slot] public static Slot Slot = new Slot(false);



        public static void AddComponents(Entity subject)
        {
            ItemBase.AddComponents(subject);

            // Item stuff
            Equippable.AddTo(subject);
            SlotComponent.AddTo(subject, Slot.Id);
        }

        public static void InitComponents(Entity subject)
        {
            subject.GetEquippable().DefaultPreset();
        }

        public static void Retouch(Entity subject)
        {
            Equippable.AssignToInventorySlotUniqueHandlerWrapper.AddTo(subject);
        }
    }

    [EntityType]
    public static class ShieldFront
    {
        public static EntityFactory Factory;
        
        public static void AddComponent(Entity subject)
        {
        }

        public static void InitComponents(Entity subject)
        {
        }

        public static void Retouch(Entity subject) {}
    }

    public class ShieldComponent : IComponent
    {
        [Inject] public IntVector2 _relativeDirection;
        [Inject] public int _pierceIncrease;

        private IntVector2 GetRotatedRelativeOrientation(Entity actor)
        {
            var angle = actor.GetTransform().orientation.AngleTo(IntVector2.Right);
            return _relativeDirection.Rotate(angle);
        }

        public static bool TryGetShieldComponent(Entity actor, out ShieldComponent shield)
        {
            if (actor.TryGetShield(out var shieldItem))
            {
                shield = shieldItem.GetShieldComponent();
                return true;
            }
            shield = null;
            return false;
        }

        [Handlers(nameof(BlockDirection), nameof(AbsorbDamageAndBreak))]
        public static HandlerGroup<Attackable.Context> HandlerGroup;


        [Export(Chain = "Attackable.Do")]
        public static void BlockDirection(Attackable.Context ctx)
        {
            if (TryGetShieldComponent(ctx.actor, out var shield)
                && ctx.direction == -shield.GetRotatedRelativeOrientation(ctx.actor))
            {
                ctx.resistance.pierce += shield._pierceIncrease;
            }
        }

        [Export(Chain = "Attackable.Do")]
        private void AbsorbDamageAndBreak(Attackable.Context ctx)
        {
            if (ctx.attack.damage > 0
                && ctx.actor.TryGetInventory(out var inventory)
                && inventory.TryGetShield(out var shieldItem))
            {
                var shieldComponent = shieldItem.GetShieldComponent();

                if (ctx.direction == -shieldComponent.GetRotatedRelativeOrientation(ctx.actor))
                {
                    HandlerGroup.RemoveFrom(ctx.actor);
                    inventory.RemoveFromSlot(Shield.Slot.Id);
                    ctx.attack.damage = 0;
                }
            }
        }
    }
}