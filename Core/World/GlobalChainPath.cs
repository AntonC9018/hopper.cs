using Hopper.Utils.Chains;
using Hopper.Core.WorldNS;
using Hopper.Core.Components;

namespace Hopper.Core.WorldNS
{
    public readonly struct GlobalChainPath<T> where T : IChain
    {
        public readonly Index<T> Index;

        public GlobalChainPath(Index<T> index)
        {
            Index = index;
        }

        public T Get(World world) => world.Chains.GetLazy(Index);
        public T Get() => Get(World.Global);
    }
}