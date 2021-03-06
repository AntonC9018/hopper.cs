using System.Runtime.Serialization;
using Hopper.Utils.Chains;
using Hopper.Core.Items;
using Hopper.Shared.Attributes;
using System.Collections.Generic;
using Hopper.Utils;

namespace Hopper.Core.Components.Basic
{
    public class ChainsBuilder : Dictionary<Identifier, IChain>
    {
        public ChainsBuilder() : base()
        {
        }

        public ChainsBuilder(IDictionary<Identifier, IChain> chains) : base(chains)
        {
        }

        public void Add<T>(Index<T> index, T stat) where T : IChain
        {
            Add(index.Id, stat);
        }

        public T Get<T>(Index<T> index) where T : IChain
        {
            return (T) this[index.Id];
        }

        public bool TryGet<T>(Index<T> index, out T chain) where T : IChain
        {
            if (TryGetValue(index.Id, out var t))
            {
                chain = (T) t;
                return true;   
            }
            chain = default;
            return false;
        }
    }

    public partial class MoreChains : IComponent
    {
        [Inject] public readonly ChainsBuilder template;
        public ChainsBuilder store = new ChainsBuilder();

        /// <summary>
        /// Retrieves the specified chain.
        /// If the chain has not been loaded yet, copies it from the template.
        /// If you are sure the given chain has been loaded, or you have loaded it previously, use <c>Get()</c>.
        /// </summary>
        public T GetLazy<T>(Index<T> index) where T : IChain
        {
            if (!store.TryGet(index, out var chain))
            {
                // Double lazy loading is the simplest solution to mods adding content synchronization
                if (!template.TryGet(index, out chain))
                {
                    chain = Registry.Global.MoreChains._map.Get(index);
                }
                chain = (T) chain.Copy();
                store.Add(index.Id, chain);
            }
            return chain;
        }


        /// <summary>
        /// Retrieves the specified chain.
        /// Use this method if you are sure a given chain has already been loaded.
        /// If you are not sure whether the given chain has been loaded, use <c>GetLazy()</c>.
        /// </summary>
        public T Get<T>(Index<T> index)  where T : IChain
        {
            return (T) store[index.Id];
        }
        
        /// <summary>
        /// Returns the chain if it has been lazy loaded.
        /// If the chain on template containted at least one handler, lazy loads it and returns it.
        /// </summary>
        public bool GetIfExists<T>(Index<T> index, out T chain) where T : IChain
        {
            if (store.TryGet(index, out chain))
            {
                return true;
            }
            if (template.TryGet(index, out chain))
            {
                // Lazy load the chain if it has any handlers.
                if (!chain.IsEmpty)
                {
                    // TODO: make the chain copy-on-write.
                    chain = (T) chain.Copy();
                    store.Add(index, chain);
                    return true;
                }
            }
            chain = default;
            return false;
        }
    }

    public readonly struct MoreChainPath<T> : IPath<T> where T : IChain
    {
        public readonly Index<T> Index;

        public MoreChainPath(Index<T> index)
        {
            Index = index;
        }

        public T Get(Entity entity)
        {
            if (entity.TryGetMoreChains(out var moreChains))
                return moreChains.GetLazy(Index);
            return default;
        }

        public bool GetIfExists(Entity entity, out T chain)
        {
            if (entity.TryGetMoreChains(out var moreChains))
                return moreChains.GetIfExists(Index, out chain);

            chain = default;
            return false;
        }

        /// <summary>
        /// Returns the chain from `MoreChains` component, 
        /// but only if it has been lazy loaded or would have handlers.
        /// Returns `null` if the entity does not have `MoreChains` 
        /// or the chain is not loaded and contained no handlers.
        /// Use this if you need to pass the chain and you're not sure if any handler has been added on it.
        /// If no handlers exist on it, you'll get `null` and so both 
        /// no copy of chain will be created and the chain will not be passed in vain.
        /// </summary>
        public T GetIfExists(Entity entity)
        {
            return entity.TryGetMoreChains(out var moreChains) 
                && moreChains.GetIfExists(Index, out var chain) 
                    ? chain : default;
        }

        public override string ToString()
        {
            return $"MoreChains path to {Index}";
        }
    }
}