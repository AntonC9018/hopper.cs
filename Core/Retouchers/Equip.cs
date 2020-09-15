using Core.Behaviors;
using Core.Items;
using Utils;

namespace Core.Retouchers
{
    public static class Equip
    {
        public static Retoucher OnDisplace = Retoucher
            .SingleHandlered<Displaceable.Event>(Displaceable.Do, PickUp);

        static void PickUp(CommonEvent commonEvent)
        {
            var droppedItems = commonEvent.actor.Cell.m_entities
                .Where(i => i.Layer == Layer.DROPPED)
                .ConvertAll<DroppedItem>(i => (DroppedItem)i);

            var inv = commonEvent.actor.Inventory;
            if (inv != null)
            {
                foreach (var droppedItem in droppedItems)
                {
                    if (inv.CanEquipItem(droppedItem.Item))
                    {
                        // eventually, kill through an abstraction
                        droppedItem.Die();
                        inv.Equip(droppedItem.Item);
                    }
                }

                inv.DropExcess();
            }
        }

    }
}