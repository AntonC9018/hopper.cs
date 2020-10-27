using Core.Utils.Vector;

namespace Core
{
    public class CompositeAction : Action
    {
        Action[] m_actions;

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
    }
}