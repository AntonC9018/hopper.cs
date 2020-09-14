using System.Collections.Generic;
using System.Linq;
using Utils.CircularBuffer;

namespace Core.Items
{
    public class CircularItemContainer : IItemContainer, IResizable
    {
        CircularBuffer<Item> items;
        List<Item> excess;

        public CircularItemContainer(int size)
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
        public void Remove(Item item)
        {
            var remainingItems = items
                .Where<Item>(i => i.id != item.id)
                .ToArray<Item>();
            items = new CircularBuffer<Item>(items.Capacity, remainingItems);
        }

    }
}