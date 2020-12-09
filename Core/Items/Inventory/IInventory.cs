using System.Collections.Generic;

namespace Hopper.Core.Items
{
    public interface IInventory
    {
        void Equip(IItem item);
        void Unequip(IItem item);
        void DropExcess();
        bool CanEquipItem(IItem item);
        void Destroy(IItem item);

        T GetContainer<T>(ISlot<T> slot) where T : IItemContainer<IItem>;
        bool IsEquipped(IItem item);
        IEnumerable<IItem> AllItems { get; }
    }
}