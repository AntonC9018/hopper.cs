using Hopper.Core.Registry;

namespace Hopper.Core.Items
{
    public static class BasicSlots
    {
        public static readonly SizedSlot<CircularItemContainer<ModularWeapon>> Weapon =
            new SizedSlot<CircularItemContainer<ModularWeapon>>("weapon", 1);
        public static readonly SizedSlot<CircularItemContainer<IItem>> RangeWeapon =
            new SizedSlot<CircularItemContainer<IItem>>("range_weapon", 1);
        public static readonly SizedSlot<CircularItemContainer<ModularShovel>> Shovel =
            new SizedSlot<CircularItemContainer<ModularShovel>>("shovel", 1);
        public static readonly SimpleSlot<CounterItemContainer<IItem>> Counter =
            new SimpleSlot<CounterItemContainer<IItem>>("counter_slot");

        public static void RegisterSelf(ModRegistry registry)
        {
            Weapon.RegisterSelf(registry);
            RangeWeapon.RegisterSelf(registry);
            Shovel.RegisterSelf(registry);
            Counter.RegisterSelf(registry);
        }

        public static void Patch(PatchArea patchArea)
        {
            Weapon.Patch(patchArea);
            RangeWeapon.Patch(patchArea);
            Shovel.Patch(patchArea);
            Counter.Patch(patchArea);
        }

        public static void AfterPatch(PatchArea patchArea)
        {
        }
    }
}