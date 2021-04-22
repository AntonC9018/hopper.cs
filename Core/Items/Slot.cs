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
            IsActionMapped = IsActionMapped;
        }

        [Slot] public static Slot WeaponSlot = new Slot(IsActionMapped : false);
    }
}