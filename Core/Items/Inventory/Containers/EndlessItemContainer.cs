using System.Collections.Generic;
using Newtonsoft.Json;

namespace Core.Items
{
    public class EndelssItemContanier : IItemContainer
    {
        private List<IItem> items = new List<IItem>();
        public IEnumerable<IItem> AllItems => items;
        public List<IItem> PullOutExcess() => new List<IItem>();
        public IItem this[int index] { get => items[index]; }
        public void Insert(IItem item) => items.Add(item);
        public void Remove(IItem item) => items.Remove(item);
    }
}