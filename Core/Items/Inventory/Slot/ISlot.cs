using Hopper.Core.Registry;

namespace Hopper.Core.Items
{
    public interface ISlot<out T> : IExtendent
        where T : IItemContainer<IItem>
    {
        T CreateContainer();
    }
}