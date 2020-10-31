using Core.Utils.Vector;
using System.Collections.Generic;
using Core.Utils;

namespace Core
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

        // this is a directional block. which direction it blocks is defined
        // by which direction it is looking at
        DIRECTIONAL_WALL = 0b_0000_0001_0000_0000
    }

    public static class ExtendedLayer
    {
        public static Layer BLOCK = Layer.REAL | Layer.WALL | Layer.MISC | Layer.DIRECTIONAL_WALL;
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

        public Entity GetEntityFromLayer(Layer layer)
        {
            return m_entities.FindLast(e => (e.Layer & layer) != 0);
        }

        public List<Entity> GetAllFromLayer(Layer layer)
        {
            return m_entities.Where(e => (e.Layer & layer) != 0);
        }

        public bool HasDirectionalBlock(IntVector2 direction)
        {
            var dir = direction.Copy();
            foreach (var directionalBlock in GetAllFromLayer(Layer.DIRECTIONAL_WALL))
            {
                // block diagonal movement if corner blocks are present
                if (directionalBlock.Orientation.x == dir.x)
                {
                    dir.x = 0;
                }
                if (directionalBlock.Orientation.y == dir.y)
                {
                    dir.y = 0;
                }
                if (dir == IntVector2.Zero)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasBlock(IntVector2 direction, Layer layer)
        {
            if (Layer.DIRECTIONAL_WALL.IsOfLayer(layer))
            {
                layer -= Layer.DIRECTIONAL_WALL;
                var prevCell = m_grid.GetCellAt(-direction + m_pos);
                if (prevCell != null && prevCell.HasDirectionalBlock(direction))
                {
                    return true;
                }
                if (HasDirectionalBlock(-direction))
                {
                    return true;
                }
            }
            return GetEntityFromLayer(layer) != null;
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