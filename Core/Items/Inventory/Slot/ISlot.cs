using Hopper.Core.Registry;

namespace Hopper.Core.Items
{
    public interface ISlot<out T> : IKind, IPatch
        where T : IItemContainer<IItem>
    {
        T CreateContainer();
    }
}