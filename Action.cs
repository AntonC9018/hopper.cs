using System.Numerics;

namespace Core
{
    public class Action
    {
        public System.Func<Entity, Action, bool> Try;
        public Vector2 direction;

        public Action()
        {
        }
        public Action(Behavior beh)
        {
            Try = (Entity e, Action a) => beh.Activate(e, a, null);
        }
        public Action(System.Func<Entity, Action, bool> func)
        {
            Try = (Entity e, Action a) => func(e, a);
        }
        public Action Copy()
        {
            return new Action
            {
                Try = Try,
                direction = direction
            };
        }
        public bool Do(Entity e)
        {
            return Try(e, this);
        }
    }

    public class CompositeAction
    {
        public Action[] actions;

        public bool Try(Entity e)
        {
            foreach (var a in actions)
            {
                if (a.Do(e))
                {
                    return true;
                }
            }
            return false;
        }
    }

}