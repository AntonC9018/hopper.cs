using System.Collections.Generic;
using Core.Targeting;
using Newtonsoft.Json;

namespace Core.Items
{
    public class Inventory : IInventory
    {
        public const int WeaponSlot = 0;
        public const int ShovelSlot = 1;

        private Dictionary<int, IItemContainer> m_itemSlots;

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

        public Inventory(Entity entity)
        {
            m_itemSlots = new Dictionary<int, IItemContainer>();
            m_actor = entity;
        }

        public void Equip(IItem item)
        {
            var container = m_itemSlots[item.Slot];
            container.Insert(item);
            item.BeEquipped(m_actor);
            System.Console.WriteLine($"Picked up an item with id = {item.Id}");
        }

        public void Unequip(IItem item)
        {
            var container = m_itemSlots[item.Slot];
            container.Remove(item);
            item.BeUnequipped(m_actor);
            System.Console.WriteLine($"Dropped item with id = {item.Id}");
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

        public void AddContainer(int slotId, IItemContainer container)
        {
            if (m_itemSlots.ContainsKey(slotId))
            {
                throw new System.Exception($"Container for key {slotId} has already been defined");
            }
            m_itemSlots[slotId] = container;
        }

        public bool CanEquipItem(IItem item)
        {
            return m_itemSlots.ContainsKey(item.Slot);
        }

        public IItem GetItemFromSlot(int slotId)
        {
            return m_itemSlots[slotId][0];
        }

        public List<Target> GenerateTargets(CommonEvent commonEvent, int slotId)
        {
            var targetProvider = (IProvideTargets)GetItemFromSlot(slotId);
            if (targetProvider == null)
            {
                return new List<Target>();
            }
            return targetProvider.GetTargets(commonEvent);
        }
    }
}