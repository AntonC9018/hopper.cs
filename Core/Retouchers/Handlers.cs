using System.Linq;
using Hopper.Core.Components;
using Hopper.Utils;
using Hopper.Utils.Chains;

namespace Hopper.Core
{
    public sealed class HandlerWrapper<Context> : IHookable where Context : ContextBase
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
            chainPath.Chain(entity).Add(handler);
        }

        public bool IsHookedTo(Entity entity)
        {
            return chainPath.TryGetChain(entity, out var chain) && chain.Contains(handler);
        }

        public void TryHookTo(Entity entity)
        {
            if (chainPath.TryGetChain(entity, out var chain) && !chain.Contains(handler))
            {
                chain.Add(handler);
            }
        }

        public void UnhookFrom(Entity entity)
        {
            chainPath.Chain(entity).Remove(handler);
        }
        
        public void TryUnhookFrom(Entity entity)
        {
            if (chainPath.TryGetChain(entity, out var chain))
            {
                chain.Remove(handler);
            }
        }
    }

    public interface IHookable
    {
        void TryHookTo(Entity entity);
        void HookTo(Entity entity);
        void UnhookFrom(Entity entity);
        bool IsHookedTo(Entity entity);
        void TryUnhookFrom(Entity entity);
    }

    public static class Handlers
    {
        public static HandlerGroup<Context> CreateGroup<Context>(ChainPath<Chain<Context>> chainPath, params Handler<Context>[] handlers) where Context : ContextBase
        {
            return new HandlerGroup<Context>(chainPath, handlers);
        }
    }

    public sealed class HandlerGroup<Context> : IHookable where Context : ContextBase
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
            return chainPath.TryGetChain(entity, out var chain) 
                && chain.Contains(handlers[0]);
        }

        public void UnhookFrom(Entity entity)
        {
            var chain = chainPath.Chain(entity);

            foreach (var handler in handlers)
            {
                chain.Remove(handler);
            }
        }

        public void TryHookTo(Entity entity)
        {
            if (!chainPath.TryGetChain(entity, out var chain) 
                || chain.Contains(handlers[0]))
            {
                return;
            }
            foreach (var handler in handlers)
            {
                chain.Add(handler);
            }
        }

        public void TryUnhookFrom(Entity entity)
        {
            if (chainPath.TryGetChain(entity, out var chain))
            {
                foreach (var handler in handlers)
                {
                    chain.Remove(handler);
                }
            }
        }
    }

    public sealed class HandlerGroupsWrapper : IHookable
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

        public void TryHookTo(Entity entity)
        {
            foreach (var h in hookables)
            {
                h.TryHookTo(entity);
            }
        }

        public void TryUnhookFrom(Entity entity)
        {
            foreach (var h in hookables)
            {
                h.TryUnhookFrom(entity);
            }
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