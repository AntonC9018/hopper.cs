using System.Collections.Generic;
using System.Linq;
using Utils.CircularBuffer;

namespace Core.Items
{
    public class CircularItemContainer : IItemContainer, IResizable
    {
        CircularBuffer<IItem> items;
        List<IItem> excess;

        public CircularItemContainer(int size)
        {
            items = new CircularBuffer<IItem>(size);
            excess = new List<IItem>();
        }

        public int Size
        {
            get => items.Size;
            set
            {
                var allItems = items.ToArray();
                var remainingItems = allItems.Take(value).ToArray();
                excess.AddRange(allItems.Skip(value));
                items = new CircularBuffer<IItem>(Size, remainingItems);
            }
        }

        public List<IItem> PullOutExcess()
        {
            var exc = excess;
            excess = new List<IItem>();
            return exc;
        }
        public IItem this[int index] { get => items[index]; }
        public void Insert(IItem item)
        {
            var ex = items.PushBack(item);
            if (ex != null)
                excess.Add(ex);
        }
        public void Remove(IItem item)
        {
            var remainingItems = items
                .Where<IItem>(i => i.Id != item.Id)
                .ToArray<IItem>();
            items = new CircularBuffer<IItem>(items.Capacity, remainingItems);
        }

    }
}