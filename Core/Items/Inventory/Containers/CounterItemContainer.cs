using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hopper.Core.Items
{
    public class CounterItemContainer<T> : IItemContainer<T>
        where T : IItem
    {
        private Dictionary<T, int> itemCount = new Dictionary<T, int>();

        public IEnumerable<T> AllItems
        {
            get
            {
                foreach (var kvp in itemCount)
                {
                    for (int i = 0; i < kvp.Value; i++)
                        yield return kvp.Key;
                }
            }
        }

        public IReadOnlyList<T> PullOutExcess() => new List<T>();
        public T this[int index] => throw new System.Exception("This doesn't make sense");

        public void Insert(DecomposedItem di)
        {
            if (itemCount.ContainsKey((T)di.item))
                itemCount[(T)di.item] += di.count;
            else
                itemCount.Add((T)di.item, di.count);
        }

        public void Remove(DecomposedItem di)
        {
            int newVal = itemCount[(T)di.item] -= di.count;
            if (newVal <= 0)
            {
                itemCount.Remove((T)di.item);
            }
        }

        public bool Contains(IItem item) => itemCount.ContainsKey((T)item);
    }
}