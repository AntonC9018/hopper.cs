using Hopper.Utils.Vector;
using Hopper.Utils;
using Hopper.Core.Components;
using Hopper.Shared.Attributes;

namespace Hopper.Core
{
    public struct TransformSnapshot 
    {
        public IntVector2 position;
        public IntVector2 orientation;
    }

    public partial class Transform : IComponent
    {
        public Entity entity;
        public IntVector2 position;
        public IntVector2 orientation;
        [Inject] public Layer layer;
        

        [Alias("InitTransform")]
        public Transform Init(Entity actor, IntVector2 position, IntVector2 orientation)
        {
            this.entity = actor;
            this.position = position;
            this.orientation = orientation;
            return this;
        }

        public TransformSnapshot GetSnapshot()
        {
            return new TransformSnapshot { position = position, orientation = orientation };
        }

        public void ResetPosInGrid(Entity entity, IntVector2 newPos)
        {
            // RemoveFromGrid(entity, null);
            position = newPos;
            // ResetInGrid(entity, null);
        }

        public void RemoveFromGrid(GridManager grid)
        {
            var cell = grid.GetCellAt(position);
            bool wasRemoved = cell.m_transforms.Remove(this);
            Assert.That(wasRemoved, "Trying to remove an entity which is not in the cell is not allowed");
            cell.FireLeaveEvent(this);
        }

        public void ResetInGrid(GridManager grid)
        {
            var cell = grid.GetCellAt(position);
            cell.m_transforms.Add(this);
            cell.FireEnterEvent(this);
        }

        public bool HasBlockRelative(IntVector2 direction, Layer layer)
        {

        }

        public bool HasBlockRelative(IntVector2 direction)
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