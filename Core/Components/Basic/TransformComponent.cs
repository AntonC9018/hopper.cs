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

        private GridManager Grid => World.Global.grid;
        

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
            RemoveFromGrid();
            position = newPos;
            ResetInGrid();
        }

        public void RemoveFromGrid()
        {
            var cell = Grid.GetCellAt(position);
            bool wasRemoved = cell.Remove(this);
            Assert.That(wasRemoved, "Trying to remove an entity which is not in the cell is not allowed");
            cell.FireLeaveEvent(this);
        }

        public void ResetInGrid()
        {
            var cell = Grid.GetCellAt(position);
            cell.Add(this);
            cell.FireEnterEvent(this);
        }

        public bool HasBlockRelative(IntVector2 direction, Layer layer)
        {
            return Grid.HasBlockAt(position + direction, direction, layer);
        }

        public bool HasBlockRelative(IntVector2 direction)
        {
            return Grid.HasBlockAt(position + direction, direction, ExtendedLayer.BLOCK);
        }
        
        public IntVector2 GetPosRelative(IntVector2 offset)
        {
            return position + offset;
        }

        public Cell GetCell()
        {
            return Grid.GetCellAt(position);
        }
    }
}