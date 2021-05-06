using Hopper.Utils.Vector;

namespace Hopper.Core.WorldNS
{
    public class CellMovementContext
    {
        public Transform transform;

        public Entity actor => transform.entity;
        public IntVector2 initialPosition;
        public IntVector2 initialOrientation;
        public IntVector2 direction;
        // public bool initialAliveCondition;

        public CellMovementContext(Transform transform, IntVector2 direction)
        {
            this.transform = transform;
            this.initialPosition = transform.position;
            this.initialOrientation = transform.orientation;
            this.direction = direction;
        }

        public bool HasNotMoved()
        {
            return transform.position == initialPosition;
        }

        public bool HasMoved()
        {
            return transform.position != initialPosition;
        }

        public bool HasNotReoriented()
        {
            return transform.orientation == initialOrientation;
        }
    }
}