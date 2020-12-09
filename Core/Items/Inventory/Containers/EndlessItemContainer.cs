using System.Collections.Generic;

namespace Hopper.Core.Items
{
    public class UniqueEndelssItemContanier<T> : IItemContainer<T>
        where T : IItem
    {
        private HashSet<T> items = new HashSet<T>();
        public IEnumerable<T> AllItems => items;
        public IReadOnlyList<T> PullOutExcess() => new List<T>();
        public void Insert(DecomposedItem di) => items.Add((T)di.item);
        public void Remove(DecomposedItem di) => items.Remove((T)di.item);
        public bool Contains(IItem item) => items.Contains((T)item);
    }
}