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

        public static bool GetShovel(this IInventory inventory, out ModularShovel shovel)
        {
            var container = inventory.GetContainer(BasicSlots.Shovel);
            shovel = container[0];
            return shovel != null;
        }
    }
}