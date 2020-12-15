namespace Hopper.Core.Items
{
    public class SimpleSlot<T> : SlotBase<T> where T : IItemContainer<IItem>
    {
        public SimpleSlot(string name) : base(name)
        {
        }

        public override T CreateContainer()
        {
            return (T)System.Activator.CreateInstance(typeof(T));
        }
    }
}