using System.Collections.Generic;
using System.Linq;
using Hopper.Utils.Vector;

namespace Hopper.Core
{
    public class GridManager
    {
        private Cell[,] m_grid;

        public int Width => m_grid.GetLength(0);
        public int Height => m_grid.GetLength(1);

        public GridManager(int width, int height)
        {
            m_grid = new Cell[height, width];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    m_grid[j, i].Init();
                }
            }
        }

        public GridManager(Cell[,] grid)
        {
            m_grid = grid;
        }

        public bool IsOutOfBounds(IntVector2 pos)
        {
            return pos.y < 0 || pos.x < 0 || pos.y >= Height || pos.x >= Width;
        }

        public bool IsInBounds(IntVector2 pos)
        {
            return !IsOutOfBounds(pos);
        }

        public Cell GetCellAt(IntVector2 pos)
        {
            return m_grid[pos.y, pos.x];
        }

        public bool TryGetCell(IntVector2 pos, out Cell cell)
        {
            if (IsOutOfBounds(pos)) 
            {
                cell = new Cell();
                return false;
            }
            cell = GetCellAt(pos);
            return true;
        }

        public bool AddTransform(Transform transform)
        {
            if (IsInBounds(transform.position))
            {
                GetCellAt(transform.position).m_transforms.Add(transform);
                return true;
            }
            return false;
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
        public Transform GetTransformFromLayer(
            IntVector2 position, IntVector2 direction, Layer layer)
        {
            return GetAllFromLayer(position, direction, layer).FirstOrDefault();
        }

        public bool TryGetTransformFromLayer(
            IntVector2 position, IntVector2 direction, Layer layer, out Transform transform)
        {
            transform = GetTransformFromLayer(position, direction, layer);
            return transform != null;
        }
        
        public bool HasNoTransformAt(IntVector2 position, IntVector2 direction, Layer layer)
        {
            return GetTransformFromLayer(position, direction, layer) == null;
        }
        
        public bool HasTransformAt(IntVector2 position, IntVector2 direction, Layer layer)
        {
            return !HasNoTransformAt(position, direction, layer);
        }


        public IEnumerable<Transform> GetAllFromLayer(
            IntVector2 position, IntVector2 direction, Layer layer)
        {
            if (IsInBounds(-direction + position))
            {
                var prevCell = GetCellAt(-direction + position);
                foreach (var entity in prevCell.GetAllDirectedFromLayer(direction, layer))
                {
                    yield return entity;
                }
            }
            
            var cell = GetCellAt(-direction + position);

            for (int i = cell.m_transforms.Count - 1; i >= 0; i--)
            {
                var t = cell.m_transforms[i];
                if (t.layer.HasFlag(layer))
                {
                    if (t.entity.IsDirected() && t.orientation != -direction)
                    {
                        continue;
                    }
                    yield return t;
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
        public bool HasBlockAt(IntVector2 position, IntVector2 direction, Layer layer)
        {
            // Has directional block prev
            if (!IsOutOfBounds(position))
            {
                return GetCellAt(-direction + position).HasDirectionalBlock(direction, layer);
            }
            // Has directional block current
            var cell = GetCellAt(position);
            if (cell.HasDirectionalBlock(-direction, layer))
            {
                return true;
            }
            return cell.GetUndirectedTransformFromLayer(layer) != null;
        }
    }
}