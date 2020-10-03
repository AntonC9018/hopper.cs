using System.Collections.Generic;
using Newtonsoft.Json;

namespace Core.Items
{
    public class CounterItemContainer : IItemContainer
    {
        private Dictionary<IItem, int> itemCount = new Dictionary<IItem, int>();

        public IEnumerable<IItem> AllItems{
            get {
                foreach (var kvp in itemCount)
                {
                    for (int i = 0; i < kvp.Value; i++)
                        yield return kvp.Key;
                }
            }  
        }

        public List<IItem> PullOutExcess() => new List<IItem>();
        public IItem this[int index] => throw new System.Exception("This doesn't make sense");
        public void Insert(IItem item)
        {
            if (itemCount.ContainsKey(item))
                itemCount[item]++;
            else
                itemCount[item] = 1;
        }
        public void Remove(IItem item)
        {
            itemCount[item]--;
        }
    }
}