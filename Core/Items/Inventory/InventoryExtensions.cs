namespace Hopper.Core.Items
{
    public static class InventoryExtensions
    {
        public static bool GetWeapon(this IInventory inventory, out ModularWeapon weapon)
        {
            var container = inventory.GetContainer(BasicSlots.Weapon);
            weapon = container[0];
            return weapon != null;
        }
    }
}