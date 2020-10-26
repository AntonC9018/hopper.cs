using System;
using Core.Items;
using Utils.Vector;

namespace Core
{
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

        public event System.Action<Entity> SpawnEvent;

        private T SpawnEntityNoEvent<T>(IFactory<T> entityFactory, IntVector2 pos) where T : Entity
        {
            var entity = entityFactory.Instantiate();
            entity.Init(pos, this);
            m_grid.Reset(entity);
            return entity;
        }

        public T SpawnEntity<T>(IFactory<T> entityFactory, IntVector2 pos) where T : Entity
        {
            var entity = SpawnEntityNoEvent(entityFactory, pos);
            m_state.AddEntity(entity);
            SpawnEvent?.Invoke(entity);
            return entity;
        }

        public T SpawnPlayer<T>(IFactory<T> entityFactory, IntVector2 pos) where T : Entity
        {
            var entity = SpawnEntityNoEvent(entityFactory, pos);
            m_state.AddPlayer(entity);
            SpawnEvent?.Invoke(entity);
            return entity;
        }

        public Entity CreateDroppedItem(IItem item, IntVector2 pos)
        {
            var entity = SpawnEntityNoEvent(DroppedItem.Factory, pos);
            entity.Item = item;
            SpawnEvent?.Invoke(entity);
            return entity;
        }
    }
}