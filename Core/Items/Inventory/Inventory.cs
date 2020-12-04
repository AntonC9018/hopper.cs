using System.Collections.Generic;
using Core.Targeting;

namespace Core.Items
{
    public class Inventory : IInventory
    {
        private Dictionary<ISlot<IItem>, IItemContainer> m_itemSlots;

        private Entity m_actor;

        public IEnumerable<IItem> AllItems
        {
            get
            {
                foreach (var container in m_itemSlots.Values)
                {
                    foreach (var item in container.AllItems)
                        yield return item;
                }
            }
        }

        public Inventory(Entity entity, Dictionary<ISlot<IItem>, IItemContainer> slots)
        {
            m_itemSlots = slots;
            m_actor = entity;
        }

        public Inventory(Entity entity)
        {
            m_itemSlots = new Dictionary<ISlot<IItem>, IItemContainer>(Slot._Slots.Count);
            foreach (var slot in Slot._Slots)
            {
                m_itemSlots.Add(slot, slot.CreateContainer());
            }
            m_actor = entity;
        }

        public void Equip(IItem item)
        {
            var container = m_itemSlots[item.Slot];
            item.BeEquipped(m_actor);
            container.Insert(item.Decompose());
            System.Console.WriteLine($"Picked up item {item.Metadata.name}");
        }

        public void Unequip(IItem item)
        {
            var container = m_itemSlots[item.Slot];
            container.Remove(item.Decompose());
            item.BeUnequipped(m_actor);
            System.Console.WriteLine($"Dropped item {item.Metadata.name}");
        }

        public void Destroy(IItem item)
        {
            var container = m_itemSlots[item.Slot];
            container.Remove(item.Decompose());
            item.BeDestroyed(m_actor);
            System.Console.WriteLine($"Destroyed item {item.Metadata.name}");
        }

        public void DropExcess()
        {
            foreach (var container in m_itemSlots.Values)
            {
                var excess = container.PullOutExcess();
                foreach (IItem item in excess)
                {
                    item.BeUnequipped(m_actor);
                    System.Console.WriteLine($"Dropped excess item {item.Metadata.name}");
                }
            }
        }

        public void AddContainer<T>(Slot<T, IItem> slot) where T : IItemContainer
        {
            AddContainer(slot, (T)slot.CreateContainer());
        }

        public void AddContainer<T>(Slot<T, IItem> slot, T container) where T : IItemContainer
        {
            if (m_itemSlots.ContainsKey(slot))
            {
                throw new System.Exception($"Container for key {slot.Name} has already been defined");
            }
            m_itemSlots[slot] = container;
        }

        public bool CanEquipItem(IItem item)
        {
            return m_itemSlots.ContainsKey(item.Slot);
        }

        public T GetItemFromSlot<T>(ISlot<T> slot) where T : IItem
        {
            return (T)m_itemSlots[slot as ISlot<IItem>][0];
        }

        public bool IsEquipped(IItem item) =>
            m_itemSlots[item.Slot].Contains(item.Decompose().item);


    }
}