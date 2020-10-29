using System;
using Core.Items;
using Core.Utils.Vector;

namespace Core
{
    public delegate void BringIntoGrid();

    public class World : IHaveId
    {
        public GridManager m_grid;
        public WorldStateManager m_state;

        public static readonly int NumPhases = System.Enum.GetNames(typeof(Phase)).Length;
        public static readonly int NumLayers = System.Enum.GetNames(typeof(Layer)).Length;

        public int Id => m_id;
        private int m_id;

        public World() { }

        public World(int width, int height)
        {
            m_grid = new GridManager(width, height);
            m_state = new WorldStateManager();
            m_id = IdMap.World.Add(this);
        }

        public void Loop()
        {
            m_state.Loop();
        }

        public int GetNextTimeFrame()
        {
            return m_state.GetNextTimeFrame();
        }

        public event System.Action<Entity> SpawnEntityEvent;

        // Spawning of particles. (A `Particle` being a `Scent` without a `Logent`)
        // 
        // So there's two ways to do this:
        //
        //  1. world is aware of particles at a basic level. The viewmodel subscribes to
        //     the spawnParticle event and reaches out to its dict of handlers, ignoring the
        //     event if no handler has been found for the specified event. The ids also need
        //     to be generated and stored, but the system is similar to that of entities.
        //  
        //  2. world is not responsible for particles. As a result, each `particle spawner`
        //     defines a static event, which has as arguments all the necessary metadata and
        //     the world object (since there may be multiple worlds at a time). The code
        //     defines custom handlers, called watchers, who manage what exactly happens. 
        //
        // For now, I'm opting for the second option 
        //
        // public event System.Action<int> SpawnParticleEvent;

        private T SpawnEntityNoEvent<T>(
            IFactory<T> entityFactory, IntVector2 pos, IntVector2 orientation) where T : Entity
        {
            var entity = entityFactory.Instantiate();
            entity.Init(pos, orientation, this);
            m_grid.Reset(entity, entity.Pos);
            return entity;
        }

        public System.Action SpawnHangingEntity<T>(
            IFactory<T> entityFactory, IntVector2 pos) where T : Entity
        {
            var entity = entityFactory.Instantiate();
            entity.Init(pos, this);
            SpawnEntityEvent?.Invoke(entity);

            return () =>
            {
                m_grid.Reset(entity, entity.Pos);
            };
        }

        public T SpawnEntity<T>(
            IFactory<T> entityFactory, IntVector2 pos, IntVector2 orientation) where T : Entity
        {
            var entity = SpawnEntityNoEvent(entityFactory, pos, orientation);
            m_state.AddEntity(entity);
            SpawnEntityEvent?.Invoke(entity);
            return entity;
        }

        public T SpawnEntity<T>(
            IFactory<T> entityFactory, IntVector2 pos) where T : Entity
        {
            return SpawnEntity(entityFactory, pos, IntVector2.Zero);
        }


        public T SpawnPlayer<T>(
            IFactory<T> entityFactory, IntVector2 pos, IntVector2 orientation) where T : Entity
        {
            var entity = SpawnEntityNoEvent(entityFactory, pos, orientation);
            m_state.AddPlayer(entity);
            SpawnEntityEvent?.Invoke(entity);
            return entity;
        }

        public T SpawnPlayer<T>(
            IFactory<T> entityFactory, IntVector2 pos) where T : Entity
        {
            return SpawnPlayer(entityFactory, pos, IntVector2.Zero);
        }


        public DroppedItem SpawnDroppedItem(
            IItem item, IntVector2 pos, IntVector2 orientation)
        {
            var entity = SpawnEntityNoEvent(DroppedItem.Factory, pos, orientation);
            entity.Item = item;
            SpawnEntityEvent?.Invoke(entity);
            return entity;
        }

        public DroppedItem SpawnDroppedItem(IItem item, IntVector2 pos)
        {
            return SpawnDroppedItem(item, pos, IntVector2.Zero);
        }

        // public void SpawnParticle(int id)
        // {
        //     SpawnParticleEvent?.Invoke(id);
        // }
    }
}