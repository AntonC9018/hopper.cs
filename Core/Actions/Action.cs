using Core.Utils.Vector;

namespace Core
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
    }
}