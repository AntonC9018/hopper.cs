using System.Collections.Generic;
using System.Linq;
using CircularBuffer;
using Vector;

namespace Core.Items
{
    public class Inventory : IInventory
    {
        public const int WeaponSlot = 0;
        public const int ShovelSlot = 1;

        private Dictionary<int, IItemContainer> m_itemSlots;
        private Entity m_actor;

        public Inventory(Entity entity)
        {
            m_itemSlots = new Dictionary<int, IItemContainer>();
            m_actor = entity;
        }

        public void Equip(Item item)
        {
            var container = m_itemSlots[item.slot];
            container.Insert(item);
            item.BeEquipped(m_actor);
            System.Console.WriteLine($"Picked up an item with id = {item.id}");
        }

        public void Unequip(Item item)
        {
            var container = m_itemSlots[item.slot];
            container.Remove(item);
            item.BeUnequipped(m_actor);
            System.Console.WriteLine($"Dropped item with id = {item.id}");
        }

        public void DropExcess()
        {
            foreach (var container in m_itemSlots.Values)
            {
                var excess = container.PullOutExcess();
                foreach (Item item in excess)
                {
                    item.BeUnequipped(m_actor);
                    System.Console.WriteLine($"Dropped excess item with id = {item.id}");
                }
            }
        }

        public void AddContainer(int slotId, IItemContainer container)
        {
            if (m_itemSlots.ContainsKey(slotId))
            {
                throw new System.Exception($"Container for key {slotId} has already been defined");
            }
            m_itemSlots[slotId] = container;
        }

        public bool CanEquipItem(Item item)
        {
            return m_itemSlots.ContainsKey(item.slot);
        }

        public Item GetItemFromSlot(int slotId)
        {
            return m_itemSlots[slotId][0];
        }
    }

    // public class UnlimitedCounterItemContainer<T> : IItemContainer
    // {
    //     int count;
    //     Item item;

    //     public UnlimitedCounterItemContainer(Item item)
    //     {
    //         count = 0;
    //         this.item = item;
    //     }

    //     public List<Item> GetExcess() => new List<Item>();
    //     public Item GetItemAtIndex(int index) => item;
    //     public void Insert(Item item) => count++;
    // }
}