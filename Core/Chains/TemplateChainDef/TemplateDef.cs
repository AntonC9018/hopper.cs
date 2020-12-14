using Hopper.Core.Behaviors;
using Hopper.Utils.Chains;

namespace Hopper.Core.Chains
{
    public delegate ChainTemplate<T> BehaviorFactoryPath<T>(IProvideBehaviorFactory factoryProvider)
        where T : EventBase;
    public interface ITemplateChainDef
    {
        void AddHandlersTo(IProvideBehaviorFactory entity);
    }

    public class TemplateChainDef<Event> : ITemplateChainDef where Event : EventBase
    {
        public BehaviorFactoryPath<Event> path;
        public EvHandler<Event>[] handlers;

        public void AddHandlersTo(IProvideBehaviorFactory EntityFactory)
        {
            var chain = path(EntityFactory);
            foreach (var handler in handlers)
            {
                chain.AddHandler(handler);
            }
        }
    }
}