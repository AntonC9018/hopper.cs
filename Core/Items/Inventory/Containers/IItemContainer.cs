using System.Collections.Generic;

namespace Core.Items
{
    public interface IItemContainer
    {
        public void Insert(Item item);
        public void Remove(Item item);
        public Item this[int index] { get; }
        public List<Item> PullOutExcess();
    }
}