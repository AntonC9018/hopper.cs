using System.Linq;

namespace Core
{
    public class CompositeAction : Action
    {
        private Action[] m_actions;

        public CompositeAction(params Action[] actions)
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

        public override bool ContainsAction(System.Type type)
        {
            return m_actions.Any(a => a.ContainsAction(type));
        }

        public override bool ContainsAction(Action action)
        {
            return m_actions.Any(a => a.ContainsAction(action));
        }
    }
}