using Hopper.Utils.Vector;
using System.Collections.Generic;
using Hopper.Utils;
using System.Linq;

namespace Hopper.Core
{
    // This indicates the order in which actions are executed
    public enum Layer
    {
        REAL = 0b_0000_0001,
        MISC = 0b_0000_0010,
        WALL = 0b_0000_0100,
        PROJECTILE = 0b_0000_1000,
        GOLD = 0b_0001_0000,
        FLOOR = 0b_0010_0000,
        TRAP = 0b_0100_0000,
        DROPPED = 0b_1000_0000,
    }

    public static class ExtendedLayer
    {
        public static Layer BLOCK = Layer.REAL | Layer.WALL | Layer.MISC;
        public static Layer ABOVE = (Layer)0b_0001_0000_0000;
    }

    public static class LayerExtensions
    {
        public static bool IsOfLayer(this Entity entity, Layer layer)
        {
            return (entity.Layer & layer) != 0;
        }

        public static bool IsOfLayer(this Layer layer1, Layer layer2)
        {
            return (layer1 & layer2) != 0;
        }

        public static string GetName(this Layer layer)
        {
            return System.Enum.GetName(typeof(Layer), layer);
        }

        public static Layer ToLayer(this int num)
        {
            return (Layer)(1 << (num - 1));
        }

        public static int ToIndex(this Layer layer)
        {
            int i = 0;
            uint num = (uint)layer;

            while ((num >>= 1) != 0)
                i++;

            return i;
        }
    }

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