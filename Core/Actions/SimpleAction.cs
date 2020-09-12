using Vector;

namespace Core
{
    public class SimpleAction : Action
    {
        System.Func<Entity, Action, bool> Try;

        public SimpleAction()
        {
        }
        public SimpleAction(System.Func<Entity, Action, bool> func)
        {
            Try = func;
        }
        public override Action Copy()
        {
            return new SimpleAction
            {
                Try = Try,
                direction = direction
            };
        }
        public override bool Do(Entity e)
        {
            return Try(e, this);
        }
    }
}