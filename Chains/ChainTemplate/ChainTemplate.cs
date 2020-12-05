using System.Collections.Generic;
using Hopper.Core.Utils.MyLinkedList;

namespace Chains
{
    public interface IChainTemplate
    {
        Chain Init();
        IChainTemplate Clone();
    }

    public class ChainTemplate<Event> : IChainTemplate
        where Event : EventBase
    {
        private List<EvHandler<Event>> m_handlers;
        private bool b_areHandlersCached;

        public ChainTemplate()
        {
            m_handlers = new List<EvHandler<Event>>(8);
            b_areHandlersCached = false;
        }

        private ChainTemplate(List<EvHandler<Event>> handlers)
        {
            m_handlers = new List<EvHandler<Event>>(handlers);
        }

        public void AddHandler(EvHandler<Event> handler)
        {
            b_areHandlersCached = false;
            m_handlers.Add(handler);
        }

        public ChainTemplate<Event> AddHandler(
            System.Action<Event> handlerFunc,
            PriorityRanks priority = PriorityRanks.Default)
        {
            AddHandler(new EvHandler<Event>(handlerFunc, priority));
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
            foreach (var handler in m_handlers)
            {
                chain.AddHandler(handler);
            }
            chain.SortHandlers();

            m_handlers.TrimExcess();
            int i = m_handlers.Count - 1;
            foreach (var handler in chain.Handlers)
            {
                m_handlers[i] = (EvHandler<Event>)handler;
                i--;
            }

            b_areHandlersCached = true;

            return chain;
        }

        private Chain<Event> InitFromCache()
        {
            var linkedList = new MyLinkedList<IEvHandler<Event>>();
            foreach (var handler in m_handlers)
            {
                linkedList.AddFront(handler);
            }
            return new Chain<Event>(linkedList);
        }

        public IChainTemplate Clone()
        {
            var newTemplate = new ChainTemplate<Event>(m_handlers);
            newTemplate.b_areHandlersCached = b_areHandlersCached;
            return newTemplate;
        }
    }

}
