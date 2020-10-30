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

        // TargetEvent<T> descibes where is the attacker and in what direction it's attacking
        // Meta is a special stat used for calculating targets and conditions. e.g. for
        // getting the attack targets, meta is Attack.
        IEnumerable<T> GenerateTargets<T, M>(TargetEvent<T> targetEvent, M meta, ISlot slot)
            where T : Target, new();
        IItem GetItemFromSlot(ISlot slot);
        bool IsEquipped(IItem item);
        IEnumerable<IItem> AllItems { get; }
    }
}