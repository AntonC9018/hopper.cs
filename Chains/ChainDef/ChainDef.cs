using Core.Behaviors;
using Utils.MyLinkedList;

namespace Chains
{
    public delegate Chain<T> BehaviorPath<T>(IProvideBehavior factoryProvider)
        where T : EventBase;

    public interface IChainDef
    {
        Handle[] AddHandlers(IProvideBehavior entity);
        void RemoveHandlers(Handle[] handles, IProvideBehavior entity);
    }

    public class ChainDef<Event> : IChainDef where Event : EventBase
    {
        public BehaviorPath<Event> path;
        public IEvHandler<Event>[] handlers;

        public Handle[] AddHandlers(IProvideBehavior entity)
        {
            var chain = path(entity);
            // TODO: think if this should be the intended behavior
            // since this may be pretty dangerous at times
            if (chain == null)
            {
                return new Handle[0];
            }
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