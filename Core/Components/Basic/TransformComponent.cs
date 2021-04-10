using Hopper.Utils.Vector;
using Hopper.Utils;
using Hopper.Core.Components;

namespace Hopper.Core
{
    public partial class TransformComponent : IComponent
    {
        public IntVector2 position;
        public IntVector2 orientation;

        public void ResetPosInGrid(Entity entity, IntVector2 newPos)
        {
            // RemoveFromGrid(entity, null);
            position = newPos;
            // ResetInGrid(entity, null);
        }

        public void RemoveFromGrid(Entity entity, GridManager grid)
        {
            var cell = grid.GetCellAt(position);
            bool wasRemoved = cell.m_entities.Remove(entity);
            Assert.That(wasRemoved, "Trying to remove an entity which is not in the cell is not allowed");
            cell.FireLeaveEvent(entity);
        }

        public void ResetInGrid(Entity entity, GridManager grid)
        {
            var cell = grid.GetCellAt(position);
            cell.m_entities.Add(entity);
            cell.FireEnterEvent(entity);
        }

        public bool HasBlockRelative(IntVector2 direction, Layer layer)
        {

        }
        
        public IntVector2 GetPosRelative(IntVector2 offset)
        {

        }

        public Cell GetCell()
        {
            return null;
        }
    }
}