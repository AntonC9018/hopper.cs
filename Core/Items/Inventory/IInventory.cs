namespace Core.Items
{
    public interface IInventory
    {
        public void Equip(Item item);
        public void Unequip(Item item);
        public void DropExcess();
        public bool CanEquipItem(Item item);
        public Item GetItemFromSlot(int slotId);
    }
}