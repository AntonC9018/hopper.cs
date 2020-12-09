using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Hopper.Utils.CircularBuffer;
using Hopper.Utils;

namespace Hopper.Core.Items
{
    public class CircularItemContainer<T> : IResizableContainer<T>
        where T : IItem
    {
        private CircularBuffer<T> m_items;
        private List<T> m_excess;

        public CircularItemContainer(int size)
        {
            m_items = new CircularBuffer<T>(size);
            m_excess = new List<T>();
        }

        public int Size
        {
            get => m_items.Capacity;
            set
            {
                int i = 0;
                var newItems = new T[value];
                var e = m_items.GetEnumerator();
                int maxIters = Maths.Min(m_items.Size, value);

                while (i < maxIters && e.MoveNext())
                {
                    // will throw from here, if out of bounds
                    newItems[i++] = e.Current;
                }

                // adding the rest to excess
                if (m_items.Size > value)
                {
                    while (e.MoveNext())
                    {
                        m_excess.Add(e.Current);
                    }
                }

                m_items = new CircularBuffer<T>(value, newItems);
            }
        }

        public IEnumerable<T> AllItems => m_items.ToArray();

        public IReadOnlyList<T> PullOutExcess()
        {
            var excess = m_excess;
            m_excess = new List<T>();
            return excess;
        }

        public T this[int index] => m_items[index];
        public void Insert(DecomposedItem di)
        {
            Assert.That(di.count == 1, "Packed items are not supported in circular item containers");
            var excess = m_items.PushBack((T)di.item);
            if (excess != null)
                m_excess.Add(excess);
        }
        public void Remove(DecomposedItem di)
        {
            Assert.That(di.count == 1, "Packed items are not supported in circular item containers");
            Assert.That(m_items.Size > 0, "Removing from an empty circular item container");

            int i = 0;
            var newItems = new T[m_items.Size - 1];
            var e = m_items.GetEnumerator();
            while (e.MoveNext())
            {
                if (ReferenceEquals(e.Current, di.item))
                {
                    break;
                }
                // will throw from here, if out of bounds
                newItems[i++] = e.Current;
            }
            if (e != null)
            {
                while (e.MoveNext())
                {
                    newItems[i++] = e.Current;
                }
            }

            m_items = new CircularBuffer<T>(m_items.Capacity, newItems);
        }
        public bool Contains(IItem item) => m_items.Contains((T)item);
    }
}