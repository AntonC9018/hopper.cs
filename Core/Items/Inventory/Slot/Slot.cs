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

        public static void RegisterSelf(ModSubRegistry registry)
        {
            Weapon.RegisterSelf(registry);
            RangeWeapon.RegisterSelf(registry);
            Shovel.RegisterSelf(registry);
            Counter.RegisterSelf(registry);
        }

        public static void Patch(Repository repository)
        {
            Weapon.Patch(repository);
            RangeWeapon.Patch(repository);
            Shovel.Patch(repository);
            Counter.Patch(repository);
        }

        public static void AfterPatch(Repository repository)
        {
        }
    }
}