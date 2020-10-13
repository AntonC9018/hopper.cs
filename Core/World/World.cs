using System;
using Core.Items;
using Utils.Vector;

namespace Core
{
    public class World : IHaveId
    {
        public GridManager m_grid;
        public WorldStateManager m_state;

        public static int s_numPhases = System.Enum.GetNames(typeof(Layer)).Length;
        public static int s_numEntityTypes => s_numPhases;

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

        public Entity SpawnEntity(IEntityFactory entityFactory, IntVector2 pos)
        {
            var entity = entityFactory.Instantiate();
            entity.Init(pos, this);
            m_state.AddEntity(entity);
            m_grid.Reset(entity);
            return entity;
        }

        public Entity CreateDroppedItem(IItem item, IntVector2 pos)
        {
            var entity = (DroppedItem)SpawnEntity(DroppedItem.Factory, pos);
            entity.Item = item;
            return entity;
        }
    }
}