using System.Collections.Generic;
using Hopper.Core.Behaviors;
using Hopper.Utils.Chains;

namespace Hopper.Core.Chains
{
    public interface I_TemplateChainDef_PartBuilder
    {
        ITemplateChainDef ToStatic();
    }

    public class TemplateChainDef_PartBuilder<Event> : I_TemplateChainDef_PartBuilder where Event : EventBase
    {
        public BehaviorFactoryPath<Event> path;
        public List<Stuff<Event>> infos;
        private TemplateChainDefBuilder builder;

        public TemplateChainDef_PartBuilder(
            BehaviorFactoryPath<Event> path, TemplateChainDefBuilder builder = null)
        {
            this.path = path;
            this.builder = builder;
            infos = new List<Stuff<Event>>();
        }

        public TemplateChainDef_PartBuilder<Event> AddHandler(Stuff<Event> handler)
        {
            infos.Add(handler);
            return this;
        }

        public TemplateChainDef_PartBuilder<Event> AddHandler(System.Action<Event> handlerFunc, PriorityRank priority = PriorityRank.Default)
        {
            return AddHandler(new Stuff<Event> { handler = handlerFunc, priority = (int)priority });
        }
        public TemplateChainDef_PartBuilder<Event> AddHandler(System.Action<Event> handlerFunc, int priority)
        {
            return AddHandler(new Stuff<Event> { handler = handlerFunc, priority = priority });
        }

        public TemplateChainDefBuilder End()
        {
            return builder;
        }

        public TemplateChainDef_PartBuilder<T> AddDef<T>(IChainPaths<T> path)
            where T : EventBase
        {
            return builder.AddDef<T>(path);
        }

        public ITemplateChainDef ToStatic()
        {
            return new TemplateChainDef<Event> { path = path, handlers = infos.ToArray() };
        }
    }
}