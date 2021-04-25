using Hopper.Core.Components;
using Hopper.Utils.Chains;

namespace Hopper.Core
{
    public struct HandlerWrapper<Context> where Context : ContextBase
    {
        public Handler<Context> handler;
        public ChainPath<Chain<Context>> chainPath;

        public void AddTo(Entity entity)
        {
            var chain = chainPath.Chain(entity);
            chain.Add(handler);
        }

        public bool IsAdded(Entity entity)
        {
            var chain = chainPath.Chain(entity);
            return chain.Contains(handler);
        }

        public void RemoveFrom(Entity entity)
        {
            var chain = chainPath.Chain(entity);
            chain.Remove(handler);
        }
    }

    public struct HandlerGroup<Context> where Context : ContextBase
    {
        public Handler<Context>[] handlers;
        public ChainPath<Chain<Context>> chainPath;

        public HandlerGroup(ChainPath<Chain<Context>> chainPath, params Handler<Context>[] handlers)
        {
            this.chainPath = chainPath;
            this.handlers = handlers;
        }

        public void AddTo(Entity entity)
        {
            var chain = chainPath.Chain(entity);
            
            foreach (var handler in handlers)
            {
                chain.Add(handler);
            }
        }

        public bool IsAdded(Entity entity)
        {
            var chain = chainPath.Chain(entity);
            return chain.Contains(handlers[0]);
        }

        public void RemoveFrom(Entity entity)
        {
            var chain = chainPath.Chain(entity);

            foreach (var handler in handlers)
            {
                chain.Remove(handler);
            }
        }

        public bool TryAddTo(Entity entity)
        {
            var chain = chainPath.Chain(entity);
            if (chain.Contains(handlers[0]))
            {
                return false;
            }
            foreach (var handler in handlers)
            {
                chain.Add(handler);
            }
            return true;
        }
    }
}