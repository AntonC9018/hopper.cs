using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Behaviors;

namespace Chains
{
    public interface I_TCD_PartBuilder
    {
        ITemplateChainDef ToStatic();
    }

    public class TCD_PartBuilder<Event> : I_TCD_PartBuilder where Event : EventBase
    {
        public BehaviorFactoryPath<Event> path;
        public List<EvHandler<Event>> handlers;
        private TemplateChainDefBuilder builder;

        public TCD_PartBuilder(
            BehaviorFactoryPath<Event> path, TemplateChainDefBuilder builder = null)
        {
            this.path = path;
            this.builder = builder;
            handlers = new List<EvHandler<Event>>();
        }

        public TCD_PartBuilder<Event> AddHandler(EvHandler<Event> handler)
        {
            handlers.Add(handler);
            return this;
        }

        public TCD_PartBuilder<Event> AddHandler(System.Action<Event> handlerFunc, PriorityRanks priority = PriorityRanks.Default)
        {
            return AddHandler(new EvHandler<Event>(handlerFunc, priority));
        }

        public TemplateChainDefBuilder End()
        {
            return builder;
        }

        public TCD_PartBuilder<T> AddDef<T>(IChainPaths<T> path)
            where T : EventBase
        {
            return builder.AddDef<T>(path);
        }

        public ITemplateChainDef ToStatic()
        {
            return new TemplateChainDef<Event> { path = path, handlers = handlers.ToArray() };
        }
    }
}