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
        IEnumerable<T> GenerateTargets<T, M>(TargetEvent<T> targetEvent, M meta, int slotId)
            where T : Target, new();
        IItem GetItemFromSlot(int slotId);
        IEnumerable<IItem> AllItems { get; }
    }
}