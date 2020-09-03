using Core;
using Handle = MyLinkedList.MyListNode<Chains.IEvHandler>;

namespace Chains
{
    public interface ITemplateChainDef
    {
        public void AddHandlersTo(IProvideBehaviorFactory entity);
    }

    public class TemplateChainDef<Event> : ITemplateChainDef where Event : EventBase
    {
        public System.Func<IProvideBehaviorFactory, ChainTemplate<Event>> path;
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