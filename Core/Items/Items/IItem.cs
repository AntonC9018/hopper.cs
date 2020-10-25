namespace Core.Items
{
    public interface IItem : IHaveId, IModule
    {
        int Slot { get; }
        DecomposedItem Decompose();
    }
}