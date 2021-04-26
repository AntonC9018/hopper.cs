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

        public static void AddComponents(Entity subject, IntVector2 relativeDirection, int pierceIncrease)
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
            Equippable.AssignToInventorySlotUniqueHandlerWrapper.HookTo(subject);
        }
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

        public void 

        public void Preset()
        {

        }
    }

    public class ShieldedComponent : IComponent
    {
        
        [Export(Chain = "Attackable.Do", Dynamic = true)]
        public static void BlockDirection(Attackable.Context ctx)
        {
            if (TryGetShieldComponent(ctx.actor, out var shield)
                && ctx.direction == -shield.GetRotatedRelativeOrientation(ctx.actor))
            {
                ctx.resistance.pierce += shield._pierceIncrease;
            }
        }

        [Export(Chain = "Attackable.Do", Dynamic = true)]
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

        public void Preset(Entity entity)
        {
            ShieldComponent.BlockDirectionHandlerWrapper.AddTo(entity);
            ShieldComponent.AbsorbDamageAndBreakHandlerWrapper.AddTo(entity);
        }

        public void Unset(Entity entity)
        {
            ShieldComponent.BlockDirectionHandlerWrapper.RemoveFrom(entity);
            ShieldComponent.AbsorbDamageAndBreakHandlerWrapper.RemoveFrom(entity);
        }
    }
}