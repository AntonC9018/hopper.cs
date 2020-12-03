namespace Core
{
    public class WorldEventPath<T>
    {
        public int m_eventId;

        public WorldEventPath()
        {
            m_eventId = new WorldEvent<T>().Id;
        }

        public WorldEventPath(int eventId)
        {
            m_eventId = eventId;
        }

        public WorldEvent<T> Get(World world)
        {
            return (WorldEvent<T>)world.m_events[m_eventId];
        }

        public void Fire(World world, T pos)
        {
            Get(world).Fire(pos);
        }

        public void Subscribe(World world, System.Action<T> callback)
        {
            Get(world).Event += callback;
        }

        public void Unsubscribe(World world, System.Action<T> callback)
        {
            Get(world).Event -= callback;
        }
    }
}