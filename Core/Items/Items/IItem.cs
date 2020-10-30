namespace Core.Items
{
    public interface IItem : IHaveId, IModule
    {
        ISlot Slot { get; }
        DecomposedItem Decompose();
    }
}