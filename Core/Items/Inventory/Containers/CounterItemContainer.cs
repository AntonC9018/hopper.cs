using System.Collections.Generic;

namespace Core.Items
{
    public class CounterItemContainer : IItemContainer
    {
        Dictionary<Item, int> itemCount = new Dictionary<Item, int>();
        public List<Item> PullOutExcess() => new List<Item>();
        public Item this[int index] => throw new System.Exception("This doesn't make sense");
        public void Insert(Item item)
        {
            if (itemCount.ContainsKey(item))
                itemCount[item]++;
            else
                itemCount[item] = 1;
        }
        public void Remove(Item item)
        {
            itemCount[item]--;
        }
    }
}