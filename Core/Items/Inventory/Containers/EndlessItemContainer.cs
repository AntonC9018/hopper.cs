using System.Collections.Generic;

namespace Hopper.Core.Items
{
    public class UniqueEndelssItemContanier : IItemContainer
    {
        private HashSet<IItem> items = new HashSet<IItem>();
        public IEnumerable<IItem> AllItems => items;
        public List<IItem> PullOutExcess() => new List<IItem>();
        public IItem this[int index] { get => throw new System.Exception("Not supported"); }
        public void Insert(DecomposedItem di) => items.Add(di.item);
        public void Remove(DecomposedItem di) => items.Remove(di.item);
        public bool Contains(IItem item) => items.Contains(item);
    }
}