using System.Collections.Generic;
using MyLinkedList;

namespace Chains
{
    public class ChainTemplate<Event> where Event : EventBase
    {
        private List<EvHandler<Event>> m_handlers;

        private bool b_areHandlersCached;

        public ChainTemplate()
        {
            m_handlers = new List<EvHandler<Event>>(8);
            b_areHandlersCached = false;
        }

        private ChainTemplate(List<EvHandler<Event>> handlers, bool areHandlersCached)
        {
            m_handlers = new List<EvHandler<Event>>(handlers);
            b_areHandlersCached = areHandlersCached;
        }

        public void AddHandler(EvHandler<Event> handler)
        {
            b_areHandlersCached = false;
            m_handlers.Add(handler);
        }

        public void AddHandler(System.Action<Event> handlerFunc)
        {
            AddHandler(new EvHandler<Event>
            {
                handlerFunction = handlerFunc
            });
        }

        public Chain<Event> Init()
        {
            if (b_areHandlersCached)
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

            b_areHandlersCached = true;

            return chain;
        }


        private Chain<Event> InitFromCache()
        {
            var linkedList = new MyLinkedList<EvHandler<Event>>();
            foreach (var handler in m_handlers)
            {
                linkedList.AddFront(handler);
            }
            return new Chain<Event>(linkedList);
        }

        public ChainTemplate<Event> Clone()
        {
            return new ChainTemplate<Event>(m_handlers, b_areHandlersCached);
        }

    }

}
