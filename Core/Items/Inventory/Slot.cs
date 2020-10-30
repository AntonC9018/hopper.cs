using System.Collections.Generic;
using Core.Utils;

namespace Core.Items
{
    public interface ISlot
    {
        IItemContainer CreateContainer();
    }

    public static class Slot
    {
        public static readonly List<ISlot> _Slots = new List<ISlot>();
        public static readonly SizedSlot<CircularItemContainer> Weapon =
            new SizedSlot<CircularItemContainer>("weapon", 1);
        public static readonly SizedSlot<CircularItemContainer> RangeWeapon =
            new SizedSlot<CircularItemContainer>("range_weapon", 1);
        public static readonly SizedSlot<CircularItemContainer> Shovel =
            new SizedSlot<CircularItemContainer>("shovel", 1);
        public static readonly Slot<CounterItemContainer> Counter
            = new Slot<CounterItemContainer>("counter_slot");
    }

    public class Slot<T> : ScalableEnum, ISlot where T : IItemContainer
    {
        public Slot(string name) : base(name, Slot._Slots.Count)
        {
            Slot._Slots.Add(this);
        }

        public virtual IItemContainer CreateContainer()
        {
            return (IItemContainer)System.Activator.CreateInstance(typeof(T));
        }
    }

    public class SizedSlot<T> : Slot<T> where T : IResizableContainer
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