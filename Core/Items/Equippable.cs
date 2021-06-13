using Hopper.Core.Components;
using Hopper.Core.WorldNS;
using Hopper.Shared.Attributes;
using Hopper.Utils;

namespace Hopper.Core.Items
{
    // TODO: the typeId thing should be stored in the equippable or in another component.
    // TODO: refactor it so that the checks are done once at the start which attaches concrete handlers. 
    public partial class Equippable : IComponent
    {
        [Inject] public IHookable hookable;

        [Alias("BeEquipped")]
        public void BeEquipped(Entity actor, Entity owner, Inventory inventory)
        {
            actor.GetTransform().RemoveFromGrid();

            if (inventory.TryGetItem(actor.typeId, out var currentlyEquippedItemOfTheSameType))
            {
                if (currentlyEquippedItemOfTheSameType.TryGetCountable(out var countable))
                {
                    countable.AbsorbItem(actor);
                    return;
                }

                // The item may be replaced by itself if it is assigned a slot.
                // In that case, the logic should not be run, since it must be identical
                if (actor.TryGetSlotComponent(out var _slotComponent))
                {
                    inventory.ReplaceForSlot(_slotComponent.slotId, actor);
                    return;
                }
            }

            if (actor.TryGetSlotComponent(out var slotComponent))
            {
                BeEquippedLogic(actor, owner);
                inventory.ReplaceForSlot(slotComponent.slotId, actor);
                return;
            }

            BeEquippedLogic(actor, owner);
            inventory.Equip(actor);
        }

        [Alias("BeEquippedLogic")]
        public void BeEquippedLogic(Entity actor, Entity owner)
        {
            hookable?.HookTo(owner);
        }

        /// <summary>
        /// Unequips the item, that is, restores the item in the world.
        /// If the item has countable and the count not 1, 
        /// drops a new copy of this item in the world, having decremented the counter.
        /// Otherwise, the item is removed from the inventory and restored in the world. 
        /// </summary>
        [Alias("BeUnequipped")]
        public void BeUnequipped(Entity actor, Entity owner, Inventory inventory)
        {
            Assert.That(inventory.ContainsItem(actor.typeId));
            
            if (actor.TryGetCountable(out var countable) && countable.count > 1)
            {
                // we must create a new instance of this same type and drop it in the world
                // TODO: factor in another function that would also take amount as an argument
                var transform = owner.GetTransform();
                var factory = Registry.Global.EntityFactory.Get(actor.typeId);
                var newInstance = World.Global.SpawnEntity(
                    factory, transform.position, transform.orientation);
                countable.count--;
                return;
            }

            BeUnequippedLogic(actor, owner);
            actor.GetTransform().ResetInGrid();

            if (actor.TryGetSlotComponent(out var slotComponent))
            {
                inventory.RemoveFromSlot(slotComponent.slotId);
            }
            else
            {
                inventory.Remove(actor.typeId);
            }
        }   

        /// <summary>
        /// If the item has Countable component,
        /// decrements the count of this item, then removes the item if the count hits 0. 
        /// If the item has Slot component, removes the item from the slot.
        /// Simply removes the item if neither of those two have happened.
        /// </summary>
        [Alias("BeDestroyed")]
        public void BeDestroyed(Entity actor, Entity owner, Inventory inventory)
        {
            Assert.That(inventory.ContainsItem(actor.typeId));

            if (actor.TryGetCountable(out var countable))
            {
                countable.count--;
                if (countable.count > 0)
                {
                    return;
                }
            }

            BeUnequippedLogic(actor, owner);

            if (actor.TryGetSlotComponent(out var slotComponent))
            {
                inventory.RemoveFromSlot(slotComponent.slotId);
            }
            else
            {
                inventory.Remove(actor.typeId);                    
            }
        }

        /// <summary>
        /// Unhooks the hookable of this item from the target (for example, removes chain handlers).
        /// </summary>
        [Alias("BeUnequippedLogic")]
        public void BeUnequippedLogic(Entity actor, Entity owner)
        {
            hookable?.UnhookFrom(owner);
        }
    }
}