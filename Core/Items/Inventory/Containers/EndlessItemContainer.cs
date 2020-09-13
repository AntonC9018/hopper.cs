using System.Collections.Generic;

namespace Core.Items
{
    public class EndelssItemContanier : IItemContainer
    {
        List<Item> items = new List<Item>();
        public List<Item> PullOutExcess() => new List<Item>();
        public Item this[int index] { get => items[index]; }
        public void Insert(Item item) => items.Add(item);
        public void Remove(Item item) => items.Remove(item);
    }
}