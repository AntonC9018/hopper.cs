using System.Linq;

namespace Hopper.Core
{
    public class JoinedAction : Action
    {
        private Action[] m_actions;

        public JoinedAction(params Action[] actions)
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