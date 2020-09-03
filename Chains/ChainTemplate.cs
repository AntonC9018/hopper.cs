using System.Collections.Generic;
using Core;
using Core.Behaviors;
using MyLinkedList;

namespace Chains
{
    public abstract class ChainTemplate
    {
        protected List<IEvHandler> m_handlers;
        protected bool b_areHandlersCached;

        public ChainTemplate() { }
        protected ChainTemplate(List<IEvHandler> handlers, bool areHandlersCached)
        {
            m_handlers = new List<IEvHandler>(handlers);
            b_areHandlersCached = areHandlersCached;
        }

        public Chain Init()
        {
            if (b_areHandlersCached)
            {
                return InitFromCache();
            }
            return InitAndCache();
        }

        protected Chain _InitAndCache(Chain chain)
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
        protected abstract Chain InitFromCache();
        protected abstract Chain InitAndCache();
        public void AddHandler(IEvHandler handler)
        {
            b_areHandlersCached = false;
            m_handlers.Add(handler);
        }
        public abstract ChainTemplate Clone();
    }
    public class ChainTemplate<Event> : ChainTemplate
        where Event : EventBase
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

        public void AddHandler(System.Action<Event> handlerFunc, PRIORITY_RANKS priority = PRIORITY_RANKS.DEFAULT)
        {
            AddHandler(new EvHandler<Event>(handlerFunc, priority));
        }

        protected override Chain InitAndCache()
        {
            var chain = new Chain<Event>();
            return base._InitAndCache(chain);
        }

        protected override Chain InitFromCache()
        {
            var linkedList = new MyLinkedList<IEvHandler>();
            foreach (var handler in m_handlers)
            {
                linkedList.AddFront(handler);
            }
            return new Chain<Event>(linkedList);
        }

        public override ChainTemplate Clone()
        {
            return new ChainTemplate<Event>(m_handlers, b_areHandlersCached);
        }
    }

}
