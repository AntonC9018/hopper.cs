using Hopper.Core.Components;
using Hopper.Shared.Attributes;

namespace Hopper.Core.Items
{
    public struct Slot
    {
        public Identifier Id;
        public bool IsActionMapped;

        public Slot(bool IsActionMapped) : this()
        {
            this.IsActionMapped = IsActionMapped;
        }

        [Slot("Weapon")] public static Slot Weapon = new Slot(IsActionMapped : false);
        [Slot("Shovel")] public static Slot Shovel = new Slot(IsActionMapped : false);
    }

    public static class InventoryExtensions
    {
        // TODO: Generate these automatically!
        public static bool TryGetWeapon(this Inventory inventory, out Entity weapon)
        {
            return inventory.TryGetFromSlot(Slot.Weapon.Id, out weapon);
        } 

        public static bool TryGetShovel(this Inventory inventory, out Entity shovel)
        {
            return inventory.TryGetFromSlot(Slot.Shovel.Id, out shovel);
        } 
    }
}