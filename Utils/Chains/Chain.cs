using System.Collections.Generic;
using System.Linq;

namespace Hopper.Utils.Chains
{
    public class Chain<Context> where Context : ContextBase
    {
        internal bool m_dirty;
        internal SortedSet<Handler<Context>> m_handlers;

        public Chain()
        {
            m_handlers = new SortedSet<Handler<Context>>();
        }

        public bool Add(Handler<Context> handler)
        {
            m_dirty = true;
            return m_handlers.Add(handler);
        }

        public void PassNoCondition(Context ev)
        {
            foreach (var handler in m_handlers.ToArray())
            {
                handler.handler(ev);
            }
        }

        public void Pass(Context ev)
        {
            foreach (var handler in m_handlers.ToArray())
            {
                if (!ev.propagate)
                    return;
                handler.handler(ev);
            }
        }

        public void Pass(Context ev, System.Func<Context, bool> stopFunc)
        {
            foreach (var handler in m_handlers.ToArray())
            {
                if (stopFunc(ev))
                    return;
                handler.handler(ev);
            }
        }

        public bool Remove(Handler<Context> handler)
        {
            return m_handlers.Remove(handler);
        }

        public bool Has(Handler<Context> handler)
        {
            return m_handlers.Contains(handler);
        }
    }
}