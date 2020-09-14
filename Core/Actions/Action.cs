using Utils.Vector;

namespace Core
{
    public abstract class Action
    {
        public abstract Action Copy();
        public abstract bool Do(Entity e);
        public IntVector2 direction;

    }
}