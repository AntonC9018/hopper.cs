using System.Collections.Generic;
using MyLinkedList;

namespace Chains
{
    public class ChainTemplate<Event> where Event : EventBase
    {
        private List<WeightedEventHandler<Event>> m_handlers =
            new List<WeightedEventHandler<Event>>(8);

        private bool areHandlersCached = false;

        public void AddHandler(WeightedEventHandler<Event> handler)
        {
            areHandlersCached = false;
            m_handlers.Add(handler);
        }

        public Chain<Event> Init()
        {
            if (areHandlersCached)
            {
                return InitFromCache();
            }
            return InitAndCache();
        }

        private Chain<Event> InitAndCache()
        {
            var chain = new Chain<Event>();
            foreach (var handler in m_handlers)
            {
                chain.AddHandler(handler);
            }

            m_handlers.TrimExcess();
            int i = m_handlers.Count - 1;
            foreach (var handler in chain.m_handlers)
            {
                m_handlers[i] = handler;
                i--;
            }

            areHandlersCached = true;

            return chain;
        }


        private Chain<Event> InitFromCache()
        {
            var linkedList = new MyLinkedList<WeightedEventHandler<Event>>();
            foreach (var handler in m_handlers)
            {
                linkedList.AddFront(handler);
            }
            return new Chain<Event>(linkedList);
        }

    }

}
