using Hopper.Core.Behaviors;
using Hopper.Core.Items;
using Hopper.Utils;

namespace Hopper.Core.Retouchers
{
    public class Equip
    {
        public Retoucher OnDisplace = Retoucher
            .SingleHandlered(Displaceable.Do, PickUp);

        public void RegisterAll(Registry registry)
        {
            OnDisplace.RegisterSelf(registry);
        }

        private static void PickUp(ActorEvent actorEvent)
        {
            var droppedItems = actorEvent.actor.GetCell().m_entities
                .Where(i => i.Layer == Layer.DROPPED)
                .ConvertAll<DroppedItem>(i => (DroppedItem)i);

            var inv = actorEvent.actor.Inventory;
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