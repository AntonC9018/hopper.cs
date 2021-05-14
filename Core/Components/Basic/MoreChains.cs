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
    }

    public partial class MoreChains : IComponent
    {
        [Inject] public readonly ChainsBuilder template;
        public Dictionary<Identifier, ICopyable> store = new Dictionary<Identifier, ICopyable>();

        public T GetLazy<T>(Index<T> index) where T : IChain
        {
            if (!store.TryGetValue(index.Id, out var chain)) 
            {
                chain = template[index.Id];
                store.Add(index.Id, chain.Copy());
            }
            return (T) chain;
        }
    }

    public readonly struct MoreChainsChainPath<T> : IPath<T> where T : IChain
    {
        public readonly Index<T> Index;

        public MoreChainsChainPath(Index<T> index)
        {
            Index = index;
        }

        public T Follow(Entity entity)
        {
            return entity.GetMoreChains().GetLazy(Index);
        }
    }
}