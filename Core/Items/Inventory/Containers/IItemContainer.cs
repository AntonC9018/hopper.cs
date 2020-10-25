using System.Collections.Generic;

namespace Core.Items
{
    public interface IItemContainer
    {
        void Insert(DecomposedItem di);
        void Remove(DecomposedItem di);
        IItem this[int index] { get; }
        List<IItem> PullOutExcess();
        IEnumerable<IItem> AllItems { get; }
        bool Contains(IItem item);
    }
}