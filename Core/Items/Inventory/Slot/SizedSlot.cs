namespace Hopper.Core.Items
{
    public class SizedSlot<T> : SlotBase<T>, ISlot<T> where T : IResizableContainer<IItem>
    {
        private int m_defaultSize;
        public SizedSlot(string name, int defaultSize) : base(name)
        {
            m_defaultSize = defaultSize;
        }

        public override T CreateContainer()
        {
            return (T)System.Activator.CreateInstance(typeof(T), m_defaultSize);
        }
    }
}