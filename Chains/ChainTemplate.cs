using System.Collections.Generic;
using MyLinkedList;

namespace Chains
{
    public abstract class IChainTemplate
    {
        protected List<IEvHandler> m_handlers;
        protected bool b_areHandlersCached;

        public IChainTemplate() { }
        protected IChainTemplate(List<IEvHandler> handlers, bool areHandlersCached)
        {
            m_handlers = new List<IEvHandler>(handlers);
            b_areHandlersCached = areHandlersCached;
        }

        public IChain Init()
        {
            if (b_areHandlersCached)
            {
                return InitFromCache();
            }
            return InitAndCache();
        }

        protected IChain _InitAndCache(IChain chain)
        {
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
        protected abstract IChain InitFromCache();
        protected abstract IChain InitAndCache();
        public abstract void AddHandler(System.Action<EventBase> handlerFunc);
        public void AddHandler(IEvHandler handler)
        {
            b_areHandlersCached = false;
            m_handlers.Add(handler);
        }
        public abstract IChainTemplate Clone();



    }
    public class ChainTemplate<Event> : IChainTemplate where Event : EventBase
    {
        public ChainTemplate()
        {
            m_handlers = new List<IEvHandler>(8);
            b_areHandlersCached = false;
        }
        protected ChainTemplate(List<IEvHandler> handlers, bool areHandlersCached)
            : base(handlers, areHandlersCached)
        {
        }

        public override void AddHandler(System.Action<EventBase> handlerFunc)
        {
            AddHandler(new EvHandler<Event>(handlerFunc));
        }

        protected override IChain InitAndCache()
        {
            var chain = new Chain<Event>();
            return base._InitAndCache(chain);
        }

        protected override IChain InitFromCache()
        {
            var linkedList = new MyLinkedList<IEvHandler>();
            foreach (var handler in m_handlers)
            {
                linkedList.AddFront(handler);
            }
            return new Chain<Event>(linkedList);
        }

        public override IChainTemplate Clone()
        {
            return new ChainTemplate<Event>(m_handlers, b_areHandlersCached);
        }

    }

}
