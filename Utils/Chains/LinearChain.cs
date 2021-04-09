using System.Collections.Generic;

namespace Hopper.Utils.Chains
{
    public class LinearChain<Context>
    {
        public readonly List<System.Action<Context>> m_handlers;

        public LinearChain()
        {
            m_handlers = new List<System.Action<Context>>();
        }

        public void Add(System.Action<Context> handler)
        {
            m_handlers.Add(handler);
        }

        public void PassWithoutStop(Context ev)
        {
            foreach (var handler in m_handlers)
            {
                handler(ev);
            }
        }

        public void Pass(Context ev, System.Func<Context, bool> stopFunc)
        {
            foreach (var handler in m_handlers)
            {
                if (stopFunc(ev))
                    return;
                handler(ev);
            }
        }

        public void Clear()
        {
            m_handlers.Clear();
        }
    }
}