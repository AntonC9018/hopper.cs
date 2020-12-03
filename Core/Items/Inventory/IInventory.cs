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

        IItem GetItemFromSlot(ISlot slot);
        bool IsEquipped(IItem item);
        IEnumerable<IItem> AllItems { get; }
    }
}