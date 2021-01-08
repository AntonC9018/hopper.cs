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

    public struct Stuff<Event>
    {
        public Action<Event> handler;
        public int priority;
    }

    public class ChainTemplate<Event> : IChainTemplate
        where Event : EventBase
    {
        private List<Stuff<Event>> m_infos;
        private bool b_areHandlersCached;

        public ChainTemplate()
        {
            m_infos = new List<Stuff<Event>>(8);
            b_areHandlersCached = false;
        }

        private ChainTemplate(List<Stuff<Event>> handlers)
        {
            m_infos = new List<Stuff<Event>>(handlers);
        }

        public ChainTemplate<Event> AddHandler(System.Action<Event> handlerFunc,
            PriorityRank priority = PriorityRank.Default)
        {
            return AddHandler(handlerFunc, (int)priority);
        }

        public ChainTemplate<Event> AddHandler(System.Action<Event> handlerFunc, int priority)
        {
            b_areHandlersCached = false;
            m_infos.Add(new Stuff<Event> { handler = handlerFunc, priority = priority });
            return this;
        }

        public Chain Init()
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
            for (int j = 0; j < m_infos.Count; j++)
            {
                chain.AddHandler(m_infos[j].handler, (PriorityRank)m_infos[j].priority);
            }
            chain.SortHandlers();

            m_infos.TrimExcess();
            int i = m_infos.Count - 1;
            foreach (var node in chain.m_handlers)
            {
                m_infos[i] = new Stuff<Event>
                {
                    handler = node.item,
                    priority = chain.m_priorities[node]
                };
                i--;
            }

            b_areHandlersCached = true;

            return chain;
        }

        private Chain<Event> InitFromCache()
        {
            var chain = new Chain<Event>();
            foreach (var info in m_infos)
            {
                chain.m_handlers.AddFront(info.handler);
                chain.m_priorities.Add(chain.m_handlers.head, info.priority);
            }
            chain.m_dirty = false;
            return chain;
        }

        public IChainTemplate Clone()
        {
            var newTemplate = new ChainTemplate<Event>(m_infos);
            newTemplate.b_areHandlersCached = b_areHandlersCached;
            return newTemplate;
        }
    }

}
