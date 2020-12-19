using Hopper.Core.Registries;

namespace Hopper.Core
{
    public interface IWorldEvent : IExtendent
    {
        IWorldEvent GetCopy();
    }

    public class WorldEvent<T> : Extendent<IWorldEvent>, IWorldEvent
    {
        public event System.Action<T> Event;

        public WorldEvent()
        {
        }

        public void Fire(T pos)
        {
            Event?.Invoke(pos);
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