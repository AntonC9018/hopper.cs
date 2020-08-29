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
        public static int s_numEntityTypes = s_numPhases;

        public Entity SpawnEntity(IEntityFactory entityFactory, IntVector2 pos)
        {
            var entity = entityFactory.Instantiate();
            entity.Init(pos, this);
            m_state.AddEntity(entity);
            m_grid.Reset(entity);
            return entity;
        }

        public void CreateDroppedItem(int id, IntVector2 pos)
        {
            var entity = (DroppedItem)SpawnEntity(DroppedItem.s_factory, pos);
            entity.ItemId = id;
        }
    }
}