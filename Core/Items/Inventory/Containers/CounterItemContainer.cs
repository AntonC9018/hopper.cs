using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hopper.Core.Items
{
    public class CounterItemContainer : IItemContainer
    {
        private Dictionary<IItem, int> itemCount = new Dictionary<IItem, int>();

        public IEnumerable<IItem> AllItems
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

        public List<IItem> PullOutExcess() => new List<IItem>();
        public IItem this[int index] => throw new System.Exception("This doesn't make sense");

        public void Insert(DecomposedItem di)
        {
            if (itemCount.ContainsKey(di.item))
                itemCount[di.item] += di.count;
            else
                itemCount.Add(di.item, di.count);
        }

        public void Remove(DecomposedItem di)
        {
            int newVal = itemCount[di.item] -= di.count;
            if (newVal <= 0)
            {
                itemCount.Remove(di.item);
            }
        }

        public bool Contains(IItem item) => itemCount.ContainsKey(item);
    }
}