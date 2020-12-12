namespace Hopper.Core
{
    public interface IWorldEvent : IKind
    {
        IWorldEvent GetCopy();
    }

    public class WorldEvent<T> : IWorldEvent
    {
        private int m_id;
        public int Id => m_id;

        public event System.Action<T> Event;

        public WorldEvent()
        {

        }

        public void Fire(T pos)
        {
            Event?.Invoke(pos);
        }

        public void RegisterSelf(Registry registry)
        {
            m_id = registry.GetKindRegistry<IWorldEvent>().Add(this);
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