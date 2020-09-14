using Core.Behaviors;

namespace Chains
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

        public void AddHandlersTo(IProvideBehaviorFactory entityFactory)
        {
            var chain = path(entityFactory);
            foreach (var handler in handlers)
            {
                chain.AddHandler(handler);
            }
        }
    }
}