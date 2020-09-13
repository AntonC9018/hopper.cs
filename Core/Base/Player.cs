using Core.Items;

namespace Core
{
    public class Player : Entity
    {
        public override bool IsPlayer => true;
        public Player() : base()
        {
            var inventory = new Inventory(this);
            Inventory = inventory;
            // account for the weapon and the shovel slot
            inventory.AddContainer(Core.Items.Inventory.WeaponSlot, new CircularItemContainer(1));
            inventory.AddContainer(Core.Items.Inventory.ShovelSlot, new CircularItemContainer(1));
        }
    }
}