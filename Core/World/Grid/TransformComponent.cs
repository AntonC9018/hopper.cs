using Hopper.Utils.Vector;
using Hopper.Utils;
using Hopper.Core.Components;
using Hopper.Shared.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace Hopper.Core.WorldNS
{
    public partial class Transform : IComponent
    {
        public Entity entity;
        public IntVector2 position;
        public IntVector2 orientation;
        [Inject] public Layer layer;

        private GridManager Grid => World.Global.Grid;
        

        [Alias("InitTransform")]
        public Transform Init(Entity actor, IntVector2 position, IntVector2 orientation)
        {
            this.entity = actor;
            this.position = position;
            this.orientation = orientation;
            return this;
        }

        public void ResetPositionInGrid(IntVector2 newPos)
        {
            var direction = (newPos - position).Sign();
            RemoveFromGrid(direction);
            position = newPos;
            ResetInGrid(direction);
        }

        public void RemoveFromGrid(IntVector2 direction)
        {
            RemoveFromGrid();
            Grid.TriggerLeave(this, direction);
        }

        public void RemoveFromGrid()
        {
            var cell = Grid.GetCellAt(position);
            bool wasRemoved = cell.Remove(this);
            Assert.That(wasRemoved, "Trying to remove an entity which is not in the cell is not allowed");
        }


        public void TryRemoveFromGridWithoutEvent()
        {
            Grid.GetCellAt(position).Remove(this);
        }

        public void ResetInGrid(IntVector2 direction)
        {
            ResetInGrid();
            Grid.TriggerEnter(this, direction);
        }

        public void ResetInGrid()
        {
            var cell = Grid.GetCellAt(position);
            Assert.That(!cell.Contains(this), "Already in the cell, cannot add itself twice");
            cell.Add(this);
        }

        public bool HasBlockRelative(IntVector2 direction, Layer layer)
        {
            return Grid.HasBlockAt(position + direction, direction, layer);
        }

        public bool HasBlockRelative(IntVector2 direction)
        {
            return Grid.HasBlockAt(position + direction, direction, ExtendedLayer.BLOCK);
        }

        public IEnumerable<Transform> GetAllFromLayer(Layer layer)
        {
            return Grid.GetAllFromLayer(position, orientation, layer);
        }

        public IEnumerable<Transform> GetAllButSelfFromLayer(Layer layer)
        {
            return Grid.GetAllFromLayer(position, orientation, layer).Where(t => t != this);
        }

        public IEnumerable<Transform> GetAllUndirectedFromLayer(Layer layer)
        {
            return GetCell().GetAllUndirectedFromLayer(layer);
        }

        public IEnumerable<Transform> GetAllUndirectedButSelfFromLayer(Layer layer)
        {
            return GetCell().GetAllUndirectedFromLayer(layer).Where(t => t != this);
        }

        public IEnumerable<Transform> GetAllUndirectedButSelfFromLayerRelative(Layer layer, IntVector2 direction)
        {
            return GetCellRelative(direction).GetAllUndirectedFromLayer(layer).Where(t => t != this);
        }
        
        public IntVector2 GetRelativePosition(IntVector2 offset)
        {
            return position + offset;
        }

        public Cell GetCell()
        {
            return Grid.GetCellAt(position);
        }

        public Cell GetCellRelative(IntVector2 direction)
        {
            return Grid.GetCellAt(position + direction);
        }

        public void SubsribeToEnterEvent(System.Action<CellMovementContext> handler)
        {
            Grid.EnterTriggerGrid.Subscribe(position, handler);
        }
        
        public void SubsribeToLeaveEvent(System.Action<CellMovementContext> handler)
        {
            Grid.LeaveTriggerGrid.Subscribe(position, handler);
        }

        public void SubsribeToPermanentEnterEvent(System.Func<CellMovementContext, bool> handler)
        {
            Grid.EnterFilteringTriggerGrid.Subscribe(position, handler);
        }
        
        public void SubsribeToPermanentLeaveEvent(System.Func<CellMovementContext, bool> handler)
        {
            Grid.LeaveFilteringTriggerGrid.Subscribe(position, handler);
        }
    }
}