using System.Collections.Generic;
using Core.Targeting;

namespace Core.Items
{
    public class Inventory : IInventory
    {
        private Dictionary<ISlot, IItemContainer> m_itemSlots;

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

        public Inventory(Entity entity, Dictionary<ISlot, IItemContainer> slots)
        {
            m_itemSlots = slots;
            m_actor = entity;
        }

        public Inventory(Entity entity)
        {
            m_itemSlots = new Dictionary<ISlot, IItemContainer>(Slot._Slots.Count);
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
            System.Console.WriteLine($"Picked up an item with id = {item.Id}");
        }

        public void Unequip(IItem item)
        {
            var container = m_itemSlots[item.Slot];
            container.Remove(item.Decompose());
            item.BeUnequipped(m_actor);
            System.Console.WriteLine($"Dropped item with id = {item.Id}");
        }

        public void Destroy(IItem item)
        {
            var container = m_itemSlots[item.Slot];
            container.Remove(item.Decompose());
            item.BeDestroyed(m_actor);
            System.Console.WriteLine($"Destroyed item with id = {item.Id}");
        }

        public void DropExcess()
        {
            foreach (var container in m_itemSlots.Values)
            {
                var excess = container.PullOutExcess();
                foreach (IItem item in excess)
                {
                    item.BeUnequipped(m_actor);
                    System.Console.WriteLine($"Dropped excess item with id = {item.Id}");
                }
            }
        }

        public void AddContainer<T>(Slot<T> slot) where T : IItemContainer
        {
            AddContainer(slot, (T)slot.CreateContainer());
        }

        public void AddContainer<T>(Slot<T> slot, T container) where T : IItemContainer
        {
            if (m_itemSlots.ContainsKey(slot))
            {
                throw new System.Exception($"Container for key {slot} has already been defined");
            }
            m_itemSlots[slot] = container;
        }

        public bool CanEquipItem(IItem item)
        {
            return m_itemSlots.ContainsKey(item.Slot);
        }

        public IItem GetItemFromSlot(ISlot slot)
        {
            return m_itemSlots[slot][0];
        }

        public IEnumerable<T> GenerateTargets<T, M>(
            TargetEvent<T> targetEvent, M meta, ISlot slot)
            where T : Target, new()
        {
            var targetProvider = (ModularTargetingItem<T, M>)GetItemFromSlot(slot);
            if (targetProvider == null)
            {
                return new List<T>();
            }
            return targetProvider.GetParticularTargets(targetEvent, meta);
        }

        public bool IsEquipped(IItem item) =>
            m_itemSlots[item.Slot].Contains(item.Decompose().item);


    }
}