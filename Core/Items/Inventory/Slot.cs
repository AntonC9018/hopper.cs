using System.Collections.Generic;
using Hopper.Utils;

namespace Hopper.Core.Items
{
    public interface ISlot<out C>
        where C : IItemContainer<IItem>
    {
        C CreateContainer();
    }

    public static class Slot
    {
        public static readonly List<ISlot<IItemContainer<IItem>>> _Slots = new List<ISlot<IItemContainer<IItem>>>();
        public static readonly SizedSlot<CircularItemContainer<ModularWeapon>> Weapon =
            new SizedSlot<CircularItemContainer<ModularWeapon>>("weapon", 1);
        public static readonly SizedSlot<CircularItemContainer<IItem>> RangeWeapon =
            new SizedSlot<CircularItemContainer<IItem>>("range_weapon", 1);
        public static readonly SizedSlot<CircularItemContainer<ModularShovel>> Shovel =
            new SizedSlot<CircularItemContainer<ModularShovel>>("shovel", 1);
        public static readonly Slot<CounterItemContainer<IItem>> Counter
            = new Slot<CounterItemContainer<IItem>>("counter_slot");
    }

    public class Slot<C> : ScalableEnum, ISlot<C>
        where C : IItemContainer<IItem>
    {
        public Slot(string name) : base(name, Slot._Slots.Count)
        {
            Slot._Slots.Add(this as ISlot<IItemContainer<IItem>>);
        }

        public virtual C CreateContainer()
        {
            return (C)System.Activator.CreateInstance(typeof(C));
        }
    }

    public class SizedSlot<C> : Slot<C>
        where C : IResizableContainer<IItem>
    {
        private int m_defaultSize;

        public SizedSlot(string name, int defaultSize) : base(name)
        {
            m_defaultSize = defaultSize;
        }

        public override C CreateContainer()
        {
            return (C)System.Activator.CreateInstance(typeof(C), m_defaultSize);
        }
    }
}