namespace Hopper.Utils.Chains
{
    public class StaticChain<Event> : Chain
    {
        public readonly System.Action<Event>[] m_handlers;

        public StaticChain(params System.Action<Event>[] handlers)
        {
            m_handlers = handlers;
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
    }
}