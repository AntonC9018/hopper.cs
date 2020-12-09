using System.Collections.Generic;

namespace Hopper.Core.Items
{
    public interface IItemContainer<out T>
        where T : IItem
    {
        void Insert(DecomposedItem di);
        void Remove(DecomposedItem di);
        IReadOnlyList<T> PullOutExcess();
        IEnumerable<T> AllItems { get; }
        bool Contains(IItem item);
    }
}