using System.Collections.Generic;

namespace Hopper.Utils.Chains
{
    public class LinearChain<Event> : Chain
    {
        public readonly List<System.Action<Event>> m_handlers;

        public LinearChain()
        {
            m_handlers = new List<System.Action<Event>>();
        }

        public void AddHandler(System.Action<Event> handler)
        {
            m_handlers.Add(handler);
        }

        public void PassWithoutStop(Event ev)
        {
            foreach (var handler in m_handlers)
            {
                handler(ev);
            }
        }

        public void Pass(Event ev, System.Func<Event, bool> stopFunc)
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