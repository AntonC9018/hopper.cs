using System;
using Core.Items;
using Vector;

namespace Core
{
    public class World
    {
        public GridManager m_grid;
        public WorldStateManager m_state;

        public static int s_numPhases = System.Enum.GetNames(typeof(Layer)).Length;
        public static int s_numEntityTypes => s_numPhases;

        public void Loop()
        {
            m_state.Loop();
        }

        public Entity SpawnEntity(IInstantiateEntities entityFactory, IntVector2 pos)
        {
            var entity = entityFactory.Instantiate();
            entity.Init(pos, this);
            m_state.AddEntity(entity);
            m_grid.Reset(entity);
            return entity;
        }

        public Entity CreateDroppedItem(Item item, IntVector2 pos)
        {
            var entity = (DroppedItem)SpawnEntity(DroppedItem.s_factory, pos);
            entity.Item = item;
            return entity;
        }
    }
}