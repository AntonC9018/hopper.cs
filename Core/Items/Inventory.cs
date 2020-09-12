using System.Collections.Generic;
using System.Linq;
using CircularBuffer;
using Vector;

namespace Core.Items
{
    public interface IInventory
    {
        public void Equip(Item item);
        public void Unequip(Item item);
        public void DropExcess();
        public bool CanEquipItem(Item item);
        public Item GetItemFromSlot(int slotId);
    }
    public class Inventory : IInventory
    {
        public static int s_weaponSlot = 0;
        public static int s_shovelSlot = 1;

        Dictionary<int, IItemContainer> m_itemSlots;
        Entity m_actor;

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
            foreach (var (slot, container) in m_itemSlots)
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

    public interface IItemContainer
    {
        public void Insert(Item item);
        public void Remove(Item item);
        public Item this[int index] { get; }
        public List<Item> PullOutExcess();
    }

    public interface IResizable
    {
        public int Size { get; set; }
    }

    public class EndelssItemContanier : IItemContainer
    {
        List<Item> items = new List<Item>();
        public List<Item> PullOutExcess() => new List<Item>();
        public Item this[int index] { get => items[index]; }
        public void Insert(Item item) => items.Add(item);
        public void Remove(Item item) => items.Remove(item);
    }

    public class CyclicItemContainer : IItemContainer, IResizable
    {
        CircularBuffer<Item> items;
        List<Item> excess;

        public CyclicItemContainer(int size)
        {
            items = new CircularBuffer<Item>(size);
            excess = new List<Item>();
        }

        public int Size
        {
            get => items.Size;
            set
            {
                var allItems = items.ToArray();
                var remainingItems = allItems.Take(value).ToArray();
                excess.AddRange(allItems.Skip(value));
                items = new CircularBuffer<Item>(Size, remainingItems);
            }
        }

        public List<Item> PullOutExcess()
        {
            var exc = excess;
            excess = new List<Item>();
            return exc;
        }
        public Item this[int index] { get => items[index]; }
        public void Insert(Item item)
        {
            var ex = items.PushBack(item);
            if (ex != null)
                excess.Add(ex);
        }
        public void Remove(Item item) => throw new System.Exception("Cannot remove from cyclic buffer");

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