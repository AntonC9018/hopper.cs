using System.Collections.Generic;

namespace Core.Items
{
    public interface IInventory
    {
        void Equip(IItem item);
        void Unequip(IItem item);
        void DropExcess();
        bool CanEquipItem(IItem item);
        IEnumerable<T> GenerateTargets<T, E>(E targetEvent, int slotId)
            where T : Targeting.Target, new()
            where E : Targeting.TargetEvent<T>;
        IItem GetItemFromSlot(int slotId);
        IEnumerable<IItem> AllItems { get; }
    }
}