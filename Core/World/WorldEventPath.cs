namespace Hopper.Core
{
    public class WorldEventPath<T>
    {
        public readonly WorldEvent<T> Event;

        public WorldEventPath()
        {
            Event = new WorldEvent<T>();
        }

        public WorldEvent<T> Get(World world)
        {
            return (WorldEvent<T>)world.m_events[Event.Id];
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