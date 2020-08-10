using System.Numerics;
using System.Collections.Generic;

namespace Core
{
    public enum Layer
    {
        FLOOR = 0,
        MISC = 1,
        TRAP = 2,
        GOLD = 3,
        WALL = 4,
        PROJECTILE = 5,
        DROPPED = 6,
        REAL = 7
    }
    public class Cell
    {
        public Vector2 pos;
        public List<Entity> m_entities = new List<Entity>();

        public Entity GetFirstEntity()
        {
            return m_entities[0];
        }
        public Entity GetEntityFromLayer(Layer layer)
        {
            foreach (var e in m_entities)
            {
                if (e.m_layer == layer)
                {
                    return e;
                }
            }
            return null;
        }
    }
    public class GridManager
    {
        public Cell[,] m_grid;
        public readonly int m_width;
        public readonly int m_height;

        public GridManager(int width, int height)
        {
            m_grid = new Cell[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    m_grid[i, j] = new Cell
                    {
                        pos = new Vector2(i, j)
                    };
                }
            }
            m_width = width;
            m_height = height;
        }

        public GridManager(Cell[,] grid)
        {
            m_grid = grid;
            m_width = grid.GetLength(0);
            m_height = grid.GetLength(1);
        }

        public void Reset(Entity entity)
        {
            m_grid[(int)entity.m_pos.X, (int)entity.m_pos.Y]
                .m_entities
                .Add(entity);
        }

        public void Remove(Entity entity)
        {
            m_grid[(int)entity.m_pos.X, (int)entity.m_pos.Y]
                .m_entities
                .Remove(entity);
        }
    }
}