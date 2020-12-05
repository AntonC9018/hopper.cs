using System;
using Hopper.Utils.Vector;

namespace Hopper.Core
{
    public interface IWorldEvent : IHaveId
    {
        IWorldEvent GetCopy();
    }

    public class WorldEvent<T> : IWorldEvent
    {
        private int m_id;
        public int Id => m_id;

        public event System.Action<T> Event;

        public void Fire(T pos)
        {
            Event?.Invoke(pos);
        }

        public WorldEvent()
        {
            m_id = Registry.Default.GetKindRegistry<IWorldEvent>().Add(this);
        }

        private WorldEvent(int id)
        {
            m_id = id;
        }

        public IWorldEvent GetCopy()
        {
            return new WorldEvent<T>(m_id);
        }
    }
}