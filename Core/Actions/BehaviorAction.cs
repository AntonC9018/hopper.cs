using Core.Behaviors;
using Utils.Vector;

namespace Core
{
    public class BehaviorAction<T> : Action
        where T : Behavior, IStandartActivateable, new()
    {
        public override Action Copy()
        {
            return new BehaviorAction<T>();
        }
        public override bool Do(Entity e)
        {
            return e.GetBehavior<T>().Activate(this);
        }
    }
}