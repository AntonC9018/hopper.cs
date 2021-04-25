using Hopper.Utils.Vector;

namespace Hopper.Core
{
    public class CellMovementContext
    {
        public Transform transform;

        public Entity actor => transform.entity;
        public IntVector2 initialPosition;
        public IntVector2 initialOrientation;
        // public bool initialAliveCondition;

        public CellMovementContext(Transform transform)
        {
            this.transform = transform;
            this.initialPosition = transform.position;
            this.initialOrientation = transform.orientation;
        }

        public bool HasNotMoved()
        {
            return transform.position == initialPosition;
        }

        public bool HasNotReoriented()
        {
            return transform.orientation == initialOrientation;
        }
    }
}