using Hopper.Utils.Vector;
using Hopper.Utils;
using Hopper.Core.Components;
using Hopper.Shared.Attributes;
using System.Collections.Generic;
using System.Linq;
using Hopper.Utils.Chains;
using Hopper.Core.Components.Basic;

namespace Hopper.Core.WorldNS
{
    // TODO: Maybe merge this with Layers
    [Flags] public enum TransformFlags
    {
        /// <summary>
        /// Indicates whether the entity should trigger CellMovement enter event.
        /// This is enabled by default.
        /// </summary>
        TriggerEnterEvent = 1,


        /// <summary>
        /// Indicates whether the entity should trigger CellMovement leave event.
        /// This is enabled by default.
        /// </summary>
        TriggerLeaveEvent = 2,

        /// <summary>
        /// Indicates whether the entity takes up more than one cell.
        /// This feature is unimplemented.
        /// TODO: We need to make a bitmap that would contain the actual size too.
        /// </summary>
        Sized = 4,

        /// <summary>
        /// Indicates whether the entity occupies just one side of the cell.
        /// The side occupied is determined by the orientation.
        /// This flag is incompatible with Sized and will produce undefined behavior if used along with it.         
        /// </summary>
        Directed = 8,

        /// <summary>
        /// The default value for the flags.
        /// Implies triggering all CellMovement events, being unsized and undirected.
        /// </summary>
        Default = TriggerEnterEvent | TriggerLeaveEvent
    }

    public partial class Transform : IComponent
    {
        public Entity entity;
        public IntVector2 position;
        public IntVector2 orientation;
        [Inject] public Layers layer;
        [Inject] public TransformFlags flags;

        [Chain("+Reorient")] public static readonly Index<Chain<Transform>> ReorientIndex = new Index<Chain<Transform>>();

        private GridManager Grid => World.Global.Grid;


        [Alias("InitTransform")]
        public Transform Init(Entity actor, IntVector2 position, IntVector2 orientation)
        {
            this.entity = actor;
            this.position = position;
            this.orientation = orientation;
            return this;
        }

        public bool IsDirected() => flags.HasFlag(TransformFlags.Directed);

        public void ResetPositionInGrid(IntVector2 newPosition)
        {
            var direction = (newPosition - position).Sign();
            RemoveFromGrid(direction);
            position = newPosition;
            ResetInGrid(direction);
        }

        public void RemoveFromGrid(IntVector2 direction)
        {
            RemoveFromGrid();
            
            if (flags.HasFlag(TransformFlags.TriggerLeaveEvent))
            {
                Grid.TriggerLeave(this, direction);
            }
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

            if (flags.HasFlag(TransformFlags.TriggerEnterEvent))
            {
                Grid.TriggerEnter(this, direction);
            }
        }

        public void ResetInGrid()
        {
            var cell = Grid.GetCellAt(position);
            Assert.That(!cell.Contains(this), "Already in the cell, cannot add itself twice");
            cell.Add(this);
        }

        public void Reorient(IntVector2 newOrientation)
        {
            // I'm not sure about this at the moment
            // Assert.That(newOrientation != IntVector2.Zero);
            
            orientation = newOrientation;
            ReorientPath.GetIfExists(entity)?.Pass(this);
        }

        public bool HasBlockRelative(IntVector2 direction, Layers layer)
        {
            return Grid.HasBlockAt(position + direction, direction, layer);
        }

        public bool HasBlockRelative(IntVector2 direction)
        {
            return Grid.HasBlockAt(position + direction, direction, Layers.BLOCK);
        }

        public IEnumerable<Transform> GetAllFromLayer(Layers layer)
        {
            return Grid.GetAllFromLayer(position, orientation, layer);
        }

        public IEnumerable<Transform> GetAllButSelfFromLayer(Layers layer)
        {
            return Grid.GetAllFromLayer(position, orientation, layer).Where(t => t != this);
        }

        public IEnumerable<Transform> GetAllUndirectedFromLayer(Layers layer)
        {
            return GetCell().GetAllUndirectedFromLayer(layer);
        }

        public IEnumerable<Transform> GetAllUndirectedButSelfFromLayer(Layers layer)
        {
            return GetCell().GetAllUndirectedFromLayer(layer).Where(t => t != this);
        }

        public IEnumerable<Transform> GetAllUndirectedButSelfFromLayerRelative(Layers layer, IntVector2 direction)
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

        public void SubsribeToFilteredEnterEvent(System.Func<CellMovementContext, bool> handler)
        {
            Grid.EnterFilteringTriggerGrid.Subscribe(position, handler);
        }
        
        public void SubsribeToFilteredLeaveEvent(System.Func<CellMovementContext, bool> handler)
        {
            Grid.LeaveFilteringTriggerGrid.Subscribe(position, handler);
        }
    }
}