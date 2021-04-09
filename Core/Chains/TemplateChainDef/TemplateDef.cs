using Hopper.Core.Components;
using Hopper.Utils.Chains;

namespace Hopper.Core.Chains
{
    public delegate ChainTemplate<T> BehaviorFactoryPath<T>(IProvideBehaviorFactory factoryProvider)
        where T : ContextBase;
    public interface ITemplateChainDef
    {
        void AddHandlersTo(IProvideBehaviorFactory entity);
    }

    public class TemplateChainDef<Event> : ITemplateChainDef where Event : ContextBase
    {
        public BehaviorFactoryPath<Event> path;
        public Handler<Event>[] handlers;

        public void AddHandlersTo(IProvideBehaviorFactory EntityFactory)
        {
            var chain = path(EntityFactory);
            foreach (var info in handlers)
            {
                chain.AddHandler(info.handler, (PriorityRank)info.priority);
            }
        }
    }
}