using System.Collections.Generic;

namespace Core.Items
{
    public interface IItemContainer
    {
        void Insert(IItem item);
        void Remove(IItem item);
        IItem this[int index] { get; }
        List<IItem> PullOutExcess();
    }
}