using Hopper.Core.Utils.Vector;

namespace Hopper.Core
{
    public abstract class Action
    {
        public abstract Action Copy();
        public abstract bool Do(Entity e);
        public IntVector2 direction;

        public Action WithDir(IntVector2 direction)
        {
            this.direction = direction;
            return this;
        }

        public virtual bool ContainsAction(System.Type type)
        {
            return this.GetType() == type;
        }

        public virtual bool ContainsAction(Action action)
        {
            return this.GetType() == action.GetType();
        }
    }
}