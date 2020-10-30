using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Core.Utils.CircularBuffer;

namespace Core.Items
{
    public class CircularItemContainer : IResizableContainer
    {
        private CircularBuffer<IItem> items;
        private List<IItem> excess;

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
                var remainingItems = AllItems.Take(value).ToArray();
                excess.AddRange(AllItems.Skip(value));
                items = new CircularBuffer<IItem>(Size, remainingItems);
            }
        }

        public IEnumerable<IItem> AllItems => items.ToArray();

        public List<IItem> PullOutExcess()
        {
            var exc = excess;
            excess = new List<IItem>();
            return exc;
        }
        public IItem this[int index] { get => items[index]; }
        public void Insert(DecomposedItem di)
        {
            if (di.count != 1)
            {
                throw new System.Exception("Packed items are not supported in circular item containers");
            }
            var ex = items.PushBack(di.item);
            if (ex != null)
                excess.Add(ex);
        }
        public void Remove(DecomposedItem di)
        {
            if (di.count != 1)
            {
                throw new System.Exception("Packed items are not supported in circular item containers");
            }
            var remainingItems = items
                .Where(i => i != di.item)
                .ToArray();
            items = new CircularBuffer<IItem>(items.Capacity, remainingItems);
        }
        public bool Contains(IItem item) => items.Contains(item);
    }
}