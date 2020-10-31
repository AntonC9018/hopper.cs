using Core.Utils.Vector;
using System.Collections.Generic;

namespace Core
{
    public class GridManager
    {
        private Cell[,] m_grid;
        private readonly int m_width;
        private readonly int m_height;

        public GridManager(int width, int height)
        {
            m_grid = new Cell[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    m_grid[i, j] = new Cell(new IntVector2(j, i), this);
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
            Reset(entity, entity.Pos);
        }

        public void Reset(Entity entity, IntVector2 pos)
        {
            var cell = m_grid[pos.y, pos.x];
            cell.FireEnterEvent(entity);
            cell.m_entities.Add(entity);
        }

        public void Remove(Entity entity)
        {
            Remove(entity, entity.Pos);
        }

        public void Remove(Entity entity, IntVector2 pos)
        {
            var cell = m_grid[pos.y, pos.x];
            cell.m_entities.Remove(entity);
            cell.FireLeaveEvent(entity);
        }

        public bool IsOutOfBounds(IntVector2 pos)
        {
            return pos.y < 0 || pos.x < 0 || pos.y >= m_height || pos.x >= m_width;
        }

        public Cell GetCellAt(IntVector2 pos)
        {
            if (IsOutOfBounds(pos)) return null;
            return m_grid[pos.y, pos.x];
        }
    }
}