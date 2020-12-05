using Hopper.Utils.Vector;

namespace Hopper.Core
{
    public class SimpleAction : Action
    {
        System.Func<Entity, Action, bool> Try;

        public SimpleAction(System.Func<Entity, Action, bool> func)
        {
            Try = func;
        }
        public SimpleAction(System.Action<Entity, Action> func)
        {
            Try = (Entity e, Action a) => { func(e, a); return true; };
        }
        public override Action Copy()
        {
            return new SimpleAction(Try)
            {
                direction = direction
            };
        }
        public override bool Do(Entity e)
        {
            return Try(e, this);
        }

        public override bool ContainsAction(System.Type type)
        {
            return false;
        }

        public override bool ContainsAction(Action action)
        {
            return this == action;
            // return base.ContainsAction(action) && Try == ((SimpleAction)action).Try;
        }
    }
}