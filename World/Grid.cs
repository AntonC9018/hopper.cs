using System.Numerics;
using System.Collections.Generic;

namespace Core
{
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
            var cell = m_grid[(int)entity.m_pos.X, (int)entity.m_pos.Y];
            cell.m_entities.Add(entity);
            cell.FireEnterEvent(entity);
        }

        public void Reset(Entity entity, Vector2 pos)
        {
            var cell = m_grid[(int)pos.X, (int)pos.Y];
            cell.m_entities.Add(entity);
            cell.FireEnterEvent(entity);
        }

        public void Remove(Entity entity)
        {
            var cell = m_grid[(int)entity.m_pos.X, (int)entity.m_pos.Y];
            cell.m_entities.Remove(entity);
            cell.FireLeaveEvent(entity);
        }

        public void Remove(Entity entity, Vector2 pos)
        {
            var cell = m_grid[(int)pos.X, (int)pos.Y];
            cell.m_entities.Remove(entity);
            cell.FireLeaveEvent(entity);
        }

        public Cell GetCellAt(Vector2 pos)
        {
            return m_grid[(int)pos.X, (int)pos.Y];
        }
    }
}