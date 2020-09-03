using Core;
using Handle = MyLinkedList.MyListNode<Chains.IEvHandler>;

namespace Chains
{
    public interface IChainDef
    {
        public Handle[] AddHandlers(IProvideBehavior entity);
        public void RemoveHandlers(Handle[] handles, IProvideBehavior entity);
    }

    public class ChainDef<Event> : IChainDef where Event : EventBase
    {
        public System.Func<IProvideBehavior, Chain<Event>> path;
        public EvHandler<Event>[] handlers;

        public Handle[] AddHandlers(IProvideBehavior entity)
        {
            var chain = path(entity);
            var handles = new Handle[handlers.Length];
            for (int i = 0; i < handlers.Length; i++)
            {
                handles[i] = chain.AddHandler(handlers[i]);
            }
            return handles;
        }

        public void RemoveHandlers(Handle[] handles, IProvideBehavior entity)
        {
            var chain = path(entity);
            for (int i = 0; i < handles.Length; i++)
            {
                chain.RemoveHandler(handles[i]);
            }
        }
    }
}