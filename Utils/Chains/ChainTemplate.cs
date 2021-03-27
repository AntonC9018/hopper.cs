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

    public struct Handler<Context>
    {
        public Action<Context> handler;
        public int priority;
    }

    public class ChainTemplate<Context> : IChainTemplate
        where Context : ContextBase
    {
        private List<Handler<Context>> m_handlers;
        private bool m_areHandlersCached;

        public ChainTemplate()
        {
            m_handlers = new List<Handler<Context>>(8);
            m_areHandlersCached = false;
        }

        private ChainTemplate(List<Handler<Context>> handlers)
        {
            m_handlers = new List<Handler<Context>>(handlers);
        }

        public ChainTemplate<Context> AddHandler(System.Action<Context> handlerFunc,
            PriorityRank priority = PriorityRank.Default)
        {
            return AddHandler(handlerFunc, (int)priority);
        }

        public ChainTemplate<Context> AddHandler(System.Action<Context> handlerFunc, int priority)
        {
            m_areHandlersCached = false;
            m_handlers.Add(new Handler<Context> { handler = handlerFunc, priority = priority });
            return this;
        }

        public ChainTemplate<Context> AddHandler(Handler<Context> handler)
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

        private Chain<Context> InitAndCache()
        {
            var chain = new Chain<Context>();
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

        private Chain<Context> InitFromCache()
        {
            var chain = new Chain<Context>();
            foreach (var info in m_handlers)
            {
                chain.m_handlers.AddFront(info);
            }
            return chain;
        }

        public IChainTemplate Clone()
        {
            var newTemplate = new ChainTemplate<Context>(m_handlers);
            newTemplate.m_areHandlersCached = m_areHandlersCached;
            return newTemplate;
        }
    }

}
