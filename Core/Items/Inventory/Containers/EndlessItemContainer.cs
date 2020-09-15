using System.Collections.Generic;

namespace Core.Items
{
    public class EndelssItemContanier : IItemContainer
    {
        List<IItem> items = new List<IItem>();
        public List<IItem> PullOutExcess() => new List<IItem>();
        public IItem this[int index] { get => items[index]; }
        public void Insert(IItem item) => items.Add(item);
        public void Remove(IItem item) => items.Remove(item);
    }
}