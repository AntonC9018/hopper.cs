using System.Collections.Generic;
using Hopper.Utils;

namespace Hopper.Core.Items
{
    public interface ISlot<out T>
        where T : IItemContainer<IItem>
    {
        T CreateContainer();
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
        public static readonly Slot<CounterItemContainer<IItem>> Counter =
            new Slot<CounterItemContainer<IItem>>("counter_slot");
    }

    public class Slot<T> : ScalableEnum, ISlot<T>
        where T : IItemContainer<IItem>
    {
        public Slot(string name) : base(name, Slot._Slots.Count)
        {
            Slot._Slots.Add(this as ISlot<IItemContainer<IItem>>);
        }

        public virtual T CreateContainer()
        {
            return (T)System.Activator.CreateInstance(typeof(T));
        }
    }

    public class SizedSlot<T> : Slot<T>
        where T : IResizableContainer<IItem>
    {
        private int m_defaultSize;

        public SizedSlot(string name, int defaultSize) : base(name)
        {
            m_defaultSize = defaultSize;
        }

        public override T CreateContainer()
        {
            return (T)System.Activator.CreateInstance(typeof(T), m_defaultSize);
        }
    }
}