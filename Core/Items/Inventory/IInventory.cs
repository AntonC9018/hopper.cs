using System.Collections.Generic;

namespace Core.Items
{
    public interface IInventory
    {
        void Equip(IItem item);
        void Unequip(IItem item);
        void DropExcess();
        bool CanEquipItem(IItem item);
        List<Targeting.Target> GenerateTargets(CommonEvent commonEvent, int slotId);
        IItem GetItemFromSlot(int slotId);
        IEnumerable<IItem> AllItems { get; }
    }
}