using System.Collections.Generic;
using Hopper.Core.Behaviors;
using Hopper.Utils.Chains;

namespace Hopper.Core.Chains
{
    public interface I_ChainDef_PartBuilder
    {
        IChainDef ToStatic();
    }

    public class ChainDef_PartBuilder<Event> : I_ChainDef_PartBuilder where Event : EventBase
    {
        public BehaviorPath<Event> path;
        private ChainDefBuilder builder;
        public List<Stuff<Event>> handlers;

        public ChainDef_PartBuilder(BehaviorPath<Event> path, ChainDefBuilder builder)
        {
            this.path = path;
            this.builder = builder;
            handlers = new List<Stuff<Event>>();
        }

        public ChainDef_PartBuilder<Event> AddHandler(Stuff<Event> handler)
        {
            handlers.Add(handler);
            return this;
        }

        public ChainDef_PartBuilder<Event> AddHandler(System.Action<Event> handlerFunc, PriorityRank priority = PriorityRank.Default)
        {
            return AddHandler(new Stuff<Event> { handler = handlerFunc, priority = (int)priority });
        }

        public ChainDef_PartBuilder<Event> AddHandler(System.Action<Event> handlerFunc, int priority)
        {
            return AddHandler(new Stuff<Event> { handler = handlerFunc, priority = priority });
        }

        public IChainDef ToStatic()
        {
            return new ChainDef<Event> { path = path, infos = handlers.ToArray() };
        }

        public ChainDefBuilder End()
        {
            return builder;
        }

        public ChainDef_PartBuilder<T> AddDef<T>(IChainPaths<T> path)
            where T : EventBase
        {
            return builder.AddDef<T>(path);
        }
    }
}