using System.Collections.Generic;
using System.Linq;
using Core;

namespace Chains
{
    public interface ITemplateChainDefBuilder
    {
        public ITemplateChainDef ToStatic();
    }

    public class TemplateChainDefBuilder<Event> : ITemplateChainDefBuilder where Event : EventBase
    {
        public BehaviorFactoryPath<Event> path;
        public List<EvHandler<Event>> handlers;

        public TemplateChainDefBuilder(BehaviorFactoryPath<Event> path)
        {
            this.path = path;
            handlers = new List<EvHandler<Event>>();
        }

        public void AddHandler(EvHandler<Event> handler)
        {
            handlers.Add(handler);
        }

        public ITemplateChainDef ToStatic()
        {
            return new TemplateChainDef<Event> { path = path, handlers = handlers.ToArray() };
        }
    }
}