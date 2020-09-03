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
        public System.Func<IProvideBehavior, Chain<Event>> path;
        public List<EvHandler<Event>> handlers;

        public ChainDefBuilder(System.Func<IProvideBehavior, Chain<Event>> path)
        {
            this.path = path;
            handlers = new List<EvHandler<Event>>();
        }

        public void AddHandler(EvHandler<Event> handler)
        {
            handlers.Add(handler);
        }

        public IChainDef ToStatic()
        {
            return new ChainDef<Event> { path = path, handlers = handlers.ToArray() };
        }
    }
}