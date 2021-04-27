using System.Linq;
using Hopper.Core.Components;
using Hopper.Utils;
using Hopper.Utils.Chains;

namespace Hopper.Core
{
    public class HandlerWrapper<Context> : IHookable where Context : ContextBase
    {
        public Handler<Context> handler;
        public ChainPath<Chain<Context>> chainPath;

        public HandlerWrapper(Handler<Context> handler, ChainPath<Chain<Context>> chainPath)
        {
            this.handler = handler;
            this.chainPath = chainPath;
        }

        public void HookTo(Entity entity)
        {
            var chain = chainPath.Chain(entity);
            chain.Add(handler);
        }

        public bool IsHookedTo(Entity entity)
        {
            var chain = chainPath.Chain(entity);
            return chain.Contains(handler);
        }

        public void UnhookFrom(Entity entity)
        {
            var chain = chainPath.Chain(entity);
            chain.Remove(handler);
        }
    }

    public interface IHookable
    {
        void HookTo(Entity entity);
        void UnhookFrom(Entity entity);
        bool IsHookedTo(Entity entity);
    }

    public class HandlerGroup<Context> : IHookable where Context : ContextBase
    {
        public Handler<Context>[] handlers;
        public ChainPath<Chain<Context>> chainPath;

        public HandlerGroup(ChainPath<Chain<Context>> chainPath, params Handler<Context>[] handlers)
        {
            this.chainPath = chainPath;
            this.handlers = handlers;
        }

        public void HookTo(Entity entity)
        {
            var chain = chainPath.Chain(entity);
            
            foreach (var handler in handlers)
            {
                chain.Add(handler);
            }
        }

        public bool IsHookedTo(Entity entity)
        {
            var chain = chainPath.Chain(entity);
            return chain.Contains(handlers[0]);
        }

        public void UnhookFrom(Entity entity)
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

    public class HandlerGroupsWrapper : IHookable
    {
        public IHookable[] hookables;

        public HandlerGroupsWrapper(params IHookable[] hookable)
        {
            this.hookables = hookable;
        }

        public void HookTo(Entity entity)
        {
            Assert.That(!hookables.Any(h => h.IsHookedTo(entity)));
            foreach (var h in hookables)
            {
                h.HookTo(entity);
            }

        }

        public bool IsHookedTo(Entity entity)
        {
            return hookables.First().IsHookedTo(entity);
        }

        public void UnhookFrom(Entity entity)
        {
            Assert.That(hookables.All(h => h.IsHookedTo(entity)));
            foreach (var h in hookables)
            {
                h.UnhookFrom(entity);
            }
        }
    }
}