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
}