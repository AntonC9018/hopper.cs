using System.Collections.Generic;
using Core.Targeting;

namespace Core.Items
{
    public interface IInventory
    {
        void Equip(IItem item);
        void Unequip(IItem item);
        void DropExcess();
        bool CanEquipItem(IItem item);
        void Destroy(IItem item);

        T GetItemFromSlot<T>(ISlot<T> slot) where T : IItem;
        bool IsEquipped(IItem item);
        IEnumerable<IItem> AllItems { get; }
    }
}