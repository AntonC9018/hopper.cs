using System.Collections.Generic;
using Hopper.Utils;

namespace Hopper.Core.Items
{
    public interface ISlot<out T> where T : IItem
    {
        IItemContainer CreateContainer();
    }

    public static class Slot
    {
        public static readonly List<ISlot<IItem>> _Slots = new List<ISlot<IItem>>();
        public static readonly SizedSlot<CircularItemContainer, ModularWeapon> Weapon =
            new SizedSlot<CircularItemContainer, ModularWeapon>("weapon", 1);
        public static readonly SizedSlot<CircularItemContainer, IItem> RangeWeapon =
            new SizedSlot<CircularItemContainer, IItem>("range_weapon", 1);
        public static readonly SizedSlot<CircularItemContainer, ModularShovel> Shovel =
            new SizedSlot<CircularItemContainer, ModularShovel>("shovel", 1);
        public static readonly Slot<CounterItemContainer, IItem> Counter
            = new Slot<CounterItemContainer, IItem>("counter_slot");
    }

    public class Slot<T, U> : ScalableEnum, ISlot<U>
        where T : IItemContainer
        where U : IItem
    {
        public Slot(string name) : base(name, Slot._Slots.Count)
        {
            Slot._Slots.Add(this as ISlot<IItem>);
        }

        public virtual IItemContainer CreateContainer()
        {
            return (IItemContainer)System.Activator.CreateInstance(typeof(T));
        }
    }

    public class SizedSlot<T, U> : Slot<T, U>
        where T : IResizableContainer
        where U : IItem
    {
        private int m_defaultSize;

        public SizedSlot(string name, int defaultSize) : base(name)
        {
            m_defaultSize = defaultSize;
        }

        public override IItemContainer CreateContainer()
        {
            return (IItemContainer)System.Activator.CreateInstance(typeof(T), m_defaultSize);
        }
    }
}