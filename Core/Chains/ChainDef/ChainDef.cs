using Hopper.Core.Components;
using Hopper.Utils.Chains;

namespace Hopper.Core.Chains
{
    public delegate Chain<T> BehaviorPath<T>(IWithWithChain factoryProvider)
        where T : ContextBase;

    public interface IChainDef
    {
        Handle[] AddHandlers(IWithWithChain entity);
        void RemoveHandlers(Handle[] handles, IWithWithChain entity);
    }

    public class ChainDef<Event> : IChainDef where Event : ContextBase
    {
        public BehaviorPath<Event> path;
        public Handler<Event>[] infos;

        public Handle[] AddHandlers(IWithWithChain entity)
        {
            var chain = path(entity);
            // TODO: think if this should be the intended behavior
            // since this may be pretty dangerous at times
            if (chain == null)
            {
                return new Handle[0];
            }
            var handles = new Handle[infos.Length];
            for (int i = 0; i < infos.Length; i++)
            {
                handles[i] = chain.AddHandler(infos[i].handler, (PriorityRank)infos[i].priority);
            }
            return handles;
        }

        public void RemoveHandlers(Handle[] handles, IWithWithChain entity)
        {
            var chain = path(entity);
            for (int i = 0; i < handles.Length; i++)
            {
                chain.RemoveHandler(handles[i]);
            }
        }
    }
}