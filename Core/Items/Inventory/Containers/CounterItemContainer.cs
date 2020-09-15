using System.Collections.Generic;

namespace Core.Items
{
    public class CounterItemContainer : IItemContainer
    {
        Dictionary<IItem, int> itemCount = new Dictionary<IItem, int>();
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