using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Hopper.Core.Utils.CircularBuffer;
using System.Diagnostics;

namespace Hopper.Core.Items
{
    public class CircularItemContainer : IResizableContainer
    {
        private CircularBuffer<IItem> m_items;
        private List<IItem> m_excess;

        public CircularItemContainer(int size)
        {
            m_items = new CircularBuffer<IItem>(size);
            m_excess = new List<IItem>();
        }

        public int Size
        {
            get => m_items.Size;
            set
            {
                var remainingItems = AllItems.Take(value).ToArray();
                m_excess.AddRange(AllItems.Skip(value));
                m_items = new CircularBuffer<IItem>(Size, remainingItems);
            }
        }

        public IEnumerable<IItem> AllItems => m_items.ToArray();

        public List<IItem> PullOutExcess()
        {
            var excess = m_excess;
            m_excess = new List<IItem>();
            return excess;
        }
        public IItem this[int index] { get => m_items[index]; }
        public void Insert(DecomposedItem di)
        {
            Debug.Assert(di.count == 1, "Packed items are not supported in circular item containers");
            var excess = m_items.PushBack(di.item);
            if (excess != null)
                m_excess.Add(excess);
        }
        public void Remove(DecomposedItem di)
        {
            Debug.Assert(di.count == 1, "Packed items are not supported in circular item containers");
            var remainingItems = m_items
                .Where(i => i != di.item)
                .ToArray();
            m_items = new CircularBuffer<IItem>(m_items.Capacity, remainingItems);
        }
        public bool Contains(IItem item) => m_items.Contains(item);
    }
}