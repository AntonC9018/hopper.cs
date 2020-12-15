using Hopper.Core.Registry;

namespace Hopper.Core.Items
{
    public interface IItem : IKind, IModule
    {
        ISlot<IItemContainer<IItem>> Slot { get; } // think whether the slot should be more specific
        DecomposedItem Decompose();
        ItemMetadata Metadata { get; }
    }
}