using System.Collections.Generic;
using Core;

namespace Chains
{
    public interface IChainDefBuilder
    {
        public IChainDef ToStatic();
    }

    public class ChainDefBuilder<Event> : IChainDefBuilder where Event : EventBase
    {
        public BehaviorPath<Event> path;
        private ChainDefsBuilder builder;
        public List<EvHandler<Event>> handlers;

        public ChainDefBuilder(BehaviorPath<Event> path, ChainDefsBuilder builder)
        {
            this.path = path;
            this.builder = builder;
            handlers = new List<EvHandler<Event>>();
        }

        public ChainDefBuilder<Event> AddHandler(EvHandler<Event> handler)
        {
            handlers.Add(handler);
            return this;
        }

        public ChainDefBuilder<Event> AddHandler(System.Action<Event> handlerFunc, PriorityRanks priority = PriorityRanks.Default)
        {
            return AddHandler(new EvHandler<Event>(handlerFunc, priority));
        }

        public IChainDef ToStatic()
        {
            return new ChainDef<Event> { path = path, handlers = handlers.ToArray() };
        }

        public ChainDefsBuilder EndDef()
        {
            return builder;
        }
    }
}