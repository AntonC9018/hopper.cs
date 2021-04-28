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
            Equippable.AddTo(subject, ShieldComponent.Hookable);
            SlotComponent.AddTo(subject, Slot.Id);
        }

        public static void InitComponents(Entity subject)
        {
        }

        public static void Retouch(Entity subject)
        {
        }
    }


    public partial class ShieldComponent : IComponent
    {
        [Inject] public IntVector2 _relativeDirection;
        [Inject] public int _pierceIncrease;

        private IntVector2 GetRotatedRelativeOrientation(Entity actor)
        {
            var angle = actor.GetTransform().orientation.AngleTo(IntVector2.Right);
            return _relativeDirection.Rotate(angle);
        }

        [Export(Chain = "Attackable.Do")]
        public static void BlockDirection(Attackable.Context ctx)
        {
            if (ctx.actor.TryGetShield(out var shieldItem))
            {
                var shield = shieldItem.GetShieldComponent();

                if (ctx.direction == -shield.GetRotatedRelativeOrientation(ctx.actor))
                {
                    ctx.resistance.pierce += shield._pierceIncrease;
                }
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
                    shieldItem.BeDestroyed(ctx.actor, inventory);
                    // shieldItem.BeUnequippedLogic(ctx.actor);
                    // inventory.RemoveFromSlot(Shield.Slot.Id);
                    ctx.attack.damage = 0;
                }
            }
        }

        public static HandlerGroup<Attackable.Context> Hookable = new HandlerGroup<Attackable.Context>(
            Attackable.DoPath, BlockDirectionHandler, AbsorbDamageAndBreakHandler
        );
    }
}