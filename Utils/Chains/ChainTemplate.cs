using System;
using System.Collections.Generic;
using Hopper.Utils.MyLinkedList;

namespace Hopper.Utils.Chains
{
    public interface IChainTemplate
    {
        Chain Init();
        IChainTemplate Clone();
    }

    public struct Handler<Event>
    {
        public Action<Event> handler;
        public int priority;
    }

    public class ChainTemplate<Event> : IChainTemplate
        where Event : EventBase
    {
        private List<Handler<Event>> m_handlers;
        private bool m_areHandlersCached;

        public ChainTemplate()
        {
            m_handlers = new List<Handler<Event>>(8);
            m_areHandlersCached = false;
        }

        private ChainTemplate(List<Handler<Event>> handlers)
        {
            m_handlers = new List<Handler<Event>>(handlers);
        }

        public ChainTemplate<Event> AddHandler(System.Action<Event> handlerFunc,
            PriorityRank priority = PriorityRank.Default)
        {
            return AddHandler(handlerFunc, (int)priority);
        }

        public ChainTemplate<Event> AddHandler(System.Action<Event> handlerFunc, int priority)
        {
            m_areHandlersCached = false;
            m_handlers.Add(new Handler<Event> { handler = handlerFunc, priority = priority });
            return this;
        }

        public ChainTemplate<Event> AddHandler(Handler<Event> handler)
        {
            m_areHandlersCached = false;
            m_handlers.Add(handler);
            return this;
        }

        public Chain Init()
        {
            if (m_areHandlersCached)
            {
                return InitFromCache();
            }
            return InitAndCache();
        }

        private Chain<Event> InitAndCache()
        {
            var chain = new Chain<Event>();
            for (int j = 0; j < m_handlers.Count; j++)
            {
                chain.AddHandler(m_handlers[j]);
            }
            chain.SortHandlers();

            m_handlers.TrimExcess();
            int i = m_handlers.Count - 1;
            foreach (var item in chain.m_handlers)
            {
                m_handlers[i] = item;
                i--;
            }

            m_areHandlersCached = true;

            return chain;
        }

        private Chain<Event> InitFromCache()
        {
            var chain = new Chain<Event>();
            foreach (var info in m_handlers)
            {
                chain.m_handlers.AddFront(info);
            }
            return chain;
        }

        public IChainTemplate Clone()
        {
            var newTemplate = new ChainTemplate<Event>(m_handlers);
            newTemplate.m_areHandlersCached = m_areHandlersCached;
            return newTemplate;
        }
    }

}
