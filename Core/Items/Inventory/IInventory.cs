using System.Collections.Generic;
using Core.Targeting;
using Test;

namespace Core.Items
{
    public interface IInventory
    {
        void Equip(IItem item);
        void Unequip(IItem item);
        void DropExcess();
        bool CanEquipItem(IItem item);
        void Destroy(IItem item);

        // TargetEvent<T> descibes where is the attacker and in what direction it's attacking
        // Meta is a special stat used for calculating targets and conditions. e.g. for
        // getting the attack targets, meta is Attack.
        IEnumerable<T> GenerateTargets<T, M>(TargetEvent<T> targetEvent, M meta, int slotId)
            where T : Target, new();
        IItem GetItemFromSlot(int slotId);
        bool IsEquipped(IItem item);
        IEnumerable<IItem> AllItems { get; }
    }
}