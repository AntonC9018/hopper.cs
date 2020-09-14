using System.Collections.Generic;

namespace Core.Items
{
    public interface IItemContainer
    {
        void Insert(Item item);
        void Remove(Item item);
        Item this[int index] { get; }
        List<Item> PullOutExcess();
    }
}