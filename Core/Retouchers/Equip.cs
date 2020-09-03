using Chains;
using Core.Behaviors;
using Core.Items;
using System.Linq;

namespace Core.Retouchers
{
    public static class Equip
    {
        public static Retoucher OnDisplace = Retoucher
            .SingleHandlered<Displaceable.Event>(Displaceable.Do.TemplatePath, PickUp);

        static void PickUp(CommonEvent commonEvent)
        {
            var droppedItems = from droppedItem in commonEvent.actor.Cell.m_entities
                               where droppedItem.Layer == Layer.DROPPED
                               select (DroppedItem)droppedItem;

            var inv = commonEvent.actor.Inventory;
            if (inv != null)
            {
                foreach (var droppedItem in droppedItems)
                {
                    if (inv.CanEquipItem(droppedItem.Item))
                    {
                        // eventually, kill through an abstraction
                        droppedItem.RemoveFromGrid();
                        droppedItem.b_isDead = true;
                        inv.Equip(droppedItem.Item);
                    }
                }

                inv.DropExcess();
            }
        }

    }
}