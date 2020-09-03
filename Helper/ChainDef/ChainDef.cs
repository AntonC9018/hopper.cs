using Core;
using Handle = MyLinkedList.MyListNode<Chains.IEvHandler>;

namespace Chains
{
    public interface IChainDef
    {
        public void AddHandlersTo(IProvideBehavior entity);
        public Handle[] AddHandlersWithHandlesTo(IProvideBehavior entity);
        public void RemoveHandlersWithHandles(Handle[] handles, IProvideBehavior entity);
    }

    public class IChainDef<Event> : IChainDef where Event : EventBase
    {
        public System.Func<IProvideBehavior, ICanAddHandlers<Event>> path;
        public EvHandler<Event>[] handlers;

        public void AddHandlersTo(IProvideBehavior entity)
        {
            var chain = path(entity);
            foreach (var handler in handlers)
            {
                chain.AddHandler(handler);
            }
        }

        public Handle[] AddHandlersWithHandlesTo(IProvideBehavior entity)
        {
            var chain = (Chain<Event>)path(entity);
            var handles = new Handle[handlers.Length];
            for (int i = 0; i < handlers.Length; i++)
            {
                handles[i] = chain.AddWithHandle(handlers[i]);
            }
            return handles;
        }

        public void RemoveHandlersWithHandles(Handle[] handles, IProvideBehavior entity)
        {
            var chain = (Chain<Event>)path(entity);
            for (int i = 0; i < handles.Length; i++)
            {
                chain.RemoveHandler(handles[i]);
            }
        }
    }
}