using Hopper.Core.Registry;

namespace Hopper.Core.Items
{
    public abstract class SlotBase<T> : Extendent<ISlot<IItemContainer<IItem>>>, ISlot<T>
        where T : IItemContainer<IItem>
    {
        public string m_name;
        public SlotBase(string name)
        {
            m_name = name;
        }

        public abstract T CreateContainer();
    }
}