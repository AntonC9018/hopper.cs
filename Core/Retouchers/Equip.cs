using Hopper.Core.Components.Basic;
using Hopper.Core.Items;
using Hopper.Shared.Attributes;

namespace Hopper.Core.Retouchers
{
    public static partial class Equip
    {
        [Export(Chain = "Displaceable.After", Dynamic = true)]
        private static void OnDisplace(Inventory inventory, Transform transform)
        {
            foreach (var itemTransform in transform.GetCell().ToArray())
            {
                itemTransform.entity.TryBeEquipped(transform.entity, inventory);
            }

            foreach (var item in inventory.GetExcess())
            {
                item.GetTransform().ResetInGrid();
            }
            
            inventory.ClearExcess();
        }
    }
}