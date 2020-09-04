using Core.Behaviors;
using Vector;

namespace Core
{
    public abstract class Action
    {
        public abstract Action Copy();
        public abstract bool Do(Entity e);
        public IntVector2 direction;

    }
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

    public class CompositeAction : Action
    {
        Action[] m_actions;

        public CompositeAction(Action[] actions)
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
                a.direction = direction;
                if (a.Do(e))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class JoinedAction : Action
    {
        Action[] m_actions;

        public JoinedAction(Action[] actions)
        {
            m_actions = actions;
        }

        public override Action Copy()
        {
            return new JoinedAction(m_actions);
        }

        public override bool Do(Entity e)
        {
            bool success = false;
            foreach (var a in m_actions)
            {
                a.direction = direction;
                success = a.Do(e) || success;
            }
            return success;
        }
    }

}