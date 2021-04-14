using Hopper.Core.Components.Basic;
using Hopper.Core.Items;
using Hopper.Shared.Attributes;

namespace Hopper.Core.Retouchers
{
    public static partial class Equip
    {
        [Export(Chain = "Displaceable.Do", Dynamic = true)]
        private static void OnDisplace(Inventory inventory, Transform transform)
        {
            foreach (var droppedItem in transform.GetCell().m_transforms)
            {
                var item = droppedItem.GetItemComponent();
                if (inventory.CanEquipItem(item))
                {
                    // eventually, kill through an abstraction
                    droppedItem.Die();
                    inventory.Equip(item);
                }
            }

            inventory.DropExcess();
        }
    }
}