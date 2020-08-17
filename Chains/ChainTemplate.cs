using System.Collections.Generic;
using MyLinkedList;

namespace Chains
{
    public class ChainTemplate
    {
        private List<WeightedEventHandler> m_handlers;

        private bool b_areHandlersCached;

        public ChainTemplate()
        {
            m_handlers = new List<WeightedEventHandler>(8);
            b_areHandlersCached = false;
        }

        private ChainTemplate(List<WeightedEventHandler> handlers, bool areHandlersCached)
        {
            m_handlers = new List<WeightedEventHandler>(handlers);
            b_areHandlersCached = areHandlersCached;
        }

        public void AddHandler(WeightedEventHandler handler)
        {
            b_areHandlersCached = false;
            m_handlers.Add(handler);
        }

        public void AddHandler(System.Action<EventBase> handlerFunc)
        {
            AddHandler(new WeightedEventHandler
            {
                handlerFunction = handlerFunc
            });
        }

        public Chain Init()
        {
            if (b_areHandlersCached)
            {
                return InitFromCache();
            }
            return InitAndCache();
        }

        private Chain InitAndCache()
        {
            var chain = new Chain();
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


        private Chain InitFromCache()
        {
            var linkedList = new MyLinkedList<WeightedEventHandler>();
            foreach (var handler in m_handlers)
            {
                linkedList.AddFront(handler);
            }
            return new Chain(linkedList);
        }

        public ChainTemplate Clone()
        {
            return new ChainTemplate(m_handlers, b_areHandlersCached);
        }

    }

}
