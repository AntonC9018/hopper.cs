using System.Runtime.Serialization;
using Hopper.Utils.Chains;
using Hopper.Core.Items;
using Hopper.Shared.Attributes;
using System.Collections.Generic;
using Hopper.Utils;
using Hopper.Core.WorldNS;

namespace Hopper.Core.Components.Basic
{
    public class ChainsBuilder : Dictionary<Identifier, ICopyable>
    {
        public ChainsBuilder() : base()
        {
        }

        public ChainsBuilder(IDictionary<Identifier, ICopyable> chains) : base(chains)
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
        public Dictionary<Identifier, ICopyable> store = new Dictionary<Identifier, ICopyable>();

        public T GetLazy<T>(Index<T> index) where T : IChain
        {
            if (!store.TryGetValue(index.Id, out var chain))
            {
                // Double lazy loading is the simplest solution to mods adding content synchronyzation
                if (!template.TryGetValue(index.Id, out chain))
                {
                    chain = Registry.Global._defaultMoreChains[index.Id];
                }
                store.Add(index.Id, chain.Copy());
            }
            return (T) chain;
        }
    }

    public readonly struct MoreChainPath<T> : IPath<T> where T : IChain
    {
        public readonly Index<T> Index;

        public MoreChainPath(Index<T> index)
        {
            Index = index;
        }

        public T Follow(Entity entity)
        {
            if (entity.TryGetMoreChains(out var moreChains))
                return moreChains.GetLazy(Index);
            return default;
        }
    }


    public readonly struct GlobalChainPath<T> where T : IChain
    {
        public readonly Index<T> Index;

        public GlobalChainPath(Index<T> index)
        {
            Index = index;
        }

        public T Follow(World world) => world.Chains.GetLazy(Index);
        public T Follow() => Follow(World.Global);
    }
}