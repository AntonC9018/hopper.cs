namespace Core.Items
{
    public interface IInventory
    {
        void Equip(Item item);
        void Unequip(Item item);
        void DropExcess();
        bool CanEquipItem(Item item);
        Item GetItemFromSlot(int slotId);
    }
}