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
        public Dictionary<Identifier, IChain> store = new Dictionary<Identifier, IChain>();

        /// <summary>
        /// Retrieves the specified chain.
        /// If the chain has not been loaded yet, copies it from the template.
        /// If you are sure the given chain has been loaded, or you have loaded it previously, use <c>Get()</c>.
        /// </summary>
        public T GetLazy<T>(Index<T> index) where T : IChain
        {
            if (!store.TryGetValue(index.Id, out var chain))
            {
                // Double lazy loading is the simplest solution to mods adding content synchronization
                if (!template.TryGetValue(index.Id, out chain))
                {
                    chain = Registry.Global.MoreChains._map[index.Id];
                }
                chain = (IChain) chain.Copy();
                store.Add(index.Id, chain);
            }
            return (T) chain;
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

        public bool GetIfExists<T>(Index<T> index, out T chain) where T : IChain
        {
            if (store.TryGetValue(index.Id, out var _chain))
            {
                chain = (T) _chain;
                return true;
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

        public T GetIfExists(Entity entity)
        {
            return entity.TryGetMoreChains(out var moreChains) 
                && moreChains.GetIfExists(Index, out var chain) 
                    ? chain : default;
        }
    }
}