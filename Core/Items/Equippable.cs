using Hopper.Core.Components;
using Hopper.Shared.Attributes;

namespace Hopper.Core.Items
{
    [AutoActivation("BeEquipped")]
    public partial class Equippable : IBehavior
    {
        public class Context : ActorContext
        {
            public Entity inventoryOwner;
            public Inventory inventory;
        }

        [Export] public static void RemoveFromGrid(Transform transform)
        {
            transform.RemoveFromGrid();
        }

        [Export(Dynamic = true, Chain = "Equippable.Do")] 
        public static void AddToInventoryUnique(Context ctx)
        {
            ctx.inventory.Equip(ctx.actor);
        }

        [Export(Dynamic = true, Chain = "Equippable.Do")] 
        public static void AddToInventoryCountable(Context ctx)
        {
            if (ctx.inventory.TryGetItem(ctx.actor.typeId, out Entity existingItem))
            {
                existingItem.GetCountable().AbsorbItem(ctx.actor);
            }
            else
            {
                ctx.inventory.Equip(ctx.actor);
            }
        }

        [Export(Dynamic = true, Chain = "Equippable.Do")] 
        public static void AssignToInventorySlotUnique(Context ctx)
        {
            ctx.inventory.ReplaceForSlot(ctx.actor.GetSlotComponent().slotId, ctx.actor);
        }

        [Export(Dynamic = true, Chain = "Equippable.Do")] 
        public static void AssignToInventorySlotCountable(Context ctx)
        {
            if (ctx.inventory.TryGetItem(ctx.actor.typeId, out Entity existingItem))
            {
                existingItem.GetCountable().AbsorbItem(ctx.actor);
            }
            else
            {
                AssignToInventorySlotUnique(ctx);
            }
        }

        public void DefaultPreset()
        {
            _DoChain.Add(RemoveFromGridHandler);
        }
    }   
}