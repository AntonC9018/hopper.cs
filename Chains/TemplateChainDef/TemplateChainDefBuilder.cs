using System.Collections.Generic;
using System.Linq;
using Core;

namespace Chains
{
    public interface ITemplateChainDefBuilder
    {
        ITemplateChainDef ToStatic();
    }

    public class TemplateChainDefBuilder<Event> : ITemplateChainDefBuilder where Event : EventBase
    {
        public BehaviorFactoryPath<Event> path;
        public List<EvHandler<Event>> handlers;
        private TemplateChainDefsBuilder builder;

        public TemplateChainDefBuilder(
            BehaviorFactoryPath<Event> path, TemplateChainDefsBuilder builder = null)
        {
            this.path = path;
            this.builder = builder;
            handlers = new List<EvHandler<Event>>();
        }

        public TemplateChainDefBuilder<Event> AddHandler(EvHandler<Event> handler)
        {
            handlers.Add(handler);
            return this;
        }

        public TemplateChainDefBuilder<Event> AddHandler(System.Action<Event> handlerFunc, PriorityRanks priority = PriorityRanks.Default)
        {
            return AddHandler(new EvHandler<Event>(handlerFunc, priority));
        }

        public TemplateChainDefsBuilder EndDef()
        {
            return builder;
        }

        public ITemplateChainDef ToStatic()
        {
            return new TemplateChainDef<Event> { path = path, handlers = handlers.ToArray() };
        }
    }
}