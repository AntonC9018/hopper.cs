using Hopper.Utils.Vector;
using System.Collections.Generic;
using Hopper.Utils;
using System.Linq;

namespace Hopper.Core
{
    public class Cell
    {
        public IntVector2 m_pos;

        // the grid is needed for one thing.
        private GridManager m_grid;

        public List<Entity> m_entities = new List<Entity>();
        public event System.Action<Entity> EnterEvent;
        public event System.Action<Entity> LeaveEvent;

        public Cell(IntVector2 pos, GridManager grid)
        {
            m_pos = pos;
            m_grid = grid;
        }

        public Entity GetFirstEntity()
        {
            return m_entities[0];
        }

        public Entity GetAnyEntityFromLayer(Layer layer)
        {
            return m_entities.FindLast(e => e.IsOfLayer(layer));
        }

        public List<Entity> GetAllFromLayer(Layer layer)
        {
            return m_entities.Where(e => e.IsOfLayer(layer));
        }

        /*
                            ______________
            We are given   |              |
            the pos of     |   Entity2    |
            this final     |              |
            cell     --->  | --Barrier2-- |
                           |______________|
                           | --Barrier1-- |
                           |     ^        |
                    Cell   |     |        |
                    Outline|   Entity1    |
                           |______________|
            
            So imagine entity1 calls this method. It queries coordinates of the cell
            at the top and provides the direction. We must first go back to the cell
            this entity is at, checking if there are any barriers (directed entities).
            Second we check the barriers of the second block, which are on the opposite
            direction of the cell to the one given (the entity1 looks up, but we check
            to see if the block is down). Next we get the contents of the queried cell
            itself.

        */
        public Entity GetEntityFromLayer(IntVector2 direction, Layer layer)
        {
            return GetAllFromLayer(direction, layer).FirstOrDefault();
        }

        // this one looks for the fitting barriers
        public Entity GetDirectedEntityFromLayer(IntVector2 direction, Layer layer)
        {
            return GetAllDirectedFromLayer(direction, layer).FirstOrDefault();
        }

        public Entity GetUndirectedEntityFromLayer(Layer layer)
        {
            return m_entities.FindLast(e => e.IsOfLayer(layer) && e.IsDirected == false);
        }

        public IEnumerable<Entity> GetAllFromLayer(IntVector2 direction, Layer layer)
        {
            var prevCell = m_grid.GetCellAt(-direction + m_pos);
            if (prevCell != null)
            {
                foreach (var entity in prevCell.GetAllDirectedFromLayer(direction, layer))
                {
                    yield return entity;
                }
            }

            for (int i = m_entities.Count - 1; i >= 0; i--)
            {
                if (m_entities[i].IsOfLayer(layer))
                {
                    if (m_entities[i].IsDirected && m_entities[i].Orientation != -direction)
                    {
                        continue;
                    }
                    yield return m_entities[i];
                }
            }
        }

        public IEnumerable<Entity> GetAllDirectedFromLayer(IntVector2 direction, Layer layer)
        {
            for (int i = m_entities.Count - 1; i >= 0; i--)
            {
                if (m_entities[i].IsDirected
                    && m_entities[i].IsOfLayer(layer)
                    && m_entities[i].Orientation == direction)
                {
                    yield return m_entities[i];
                }
            }
        }

        /*
            this one is similar to the behavior described above, except in a situation like this:

            a diagonal direction
              \  |
               \ | <-- barrier 2           
            ____\|   
              ^
              |            
            barrier 1

            it would return `true`.
        */
        public bool HasBlock(IntVector2 direction, Layer layer)
        {
            var prevCell = m_grid.GetCellAt(-direction + m_pos);
            // Has directional block prev
            if (prevCell != null && prevCell.HasDirectionalBlock(direction, layer))
            {
                return true;
            }
            // Has directional block current
            if (HasDirectionalBlock(-direction, layer))
            {
                return true;
            }
            return GetUndirectedEntityFromLayer(layer) != null;
        }

        public bool HasDirectionalBlock(IntVector2 direction, Layer layer)
        {
            var dir = direction;
            foreach (var entity in m_entities)
            {
                if (entity.IsDirected && entity.IsOfLayer(layer))
                {
                    // block diagonal movement if corner barriers are present
                    if (entity.Orientation.x == dir.x)
                    {
                        dir.x = 0;
                    }
                    if (entity.Orientation.y == dir.y)
                    {
                        dir.y = 0;
                    }
                    if (dir == IntVector2.Zero)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void FireEnterEvent(Entity entity)
        {
            EnterEvent?.Invoke(entity);
        }

        public void FireLeaveEvent(Entity entity)
        {
            LeaveEvent?.Invoke(entity);
        }
    }
}