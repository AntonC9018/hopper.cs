namespace Core.Items
{

    public interface IItem : IHaveId, IModule
    {
        ISlot<IItem> Slot { get; }
        DecomposedItem Decompose();
        ItemMetadata Metadata { get; }
    }
}