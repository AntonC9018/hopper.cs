using System.Numerics;

namespace Core
{
    public abstract class Action
    {
        public abstract Action Copy();
        public abstract bool Do(Entity e);
        public Vector2 direction;

    }
    public class SimpleAction : Action
    {
        System.Func<Entity, Action, bool> Try;

        public SimpleAction()
        {
        }
        public SimpleAction(int behaviorId)
        {
            Try = (Entity e, Action a) => e.GetBehavior(behaviorId).Activate(e, a, null);
        }
        public SimpleAction(System.Func<Entity, Action, bool> func)
        {
            Try = (Entity e, Action a) => func(e, a);
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

    public class CompositeAction : Action
    {
        SimpleAction[] m_actions;

        public CompositeAction(SimpleAction[] actions)
        {
            m_actions = actions;
        }

        public override Action Copy()
        {
            return new CompositeAction(m_actions);
        }

        public override bool Do(Entity e)
        {
            foreach (var a in m_actions)
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