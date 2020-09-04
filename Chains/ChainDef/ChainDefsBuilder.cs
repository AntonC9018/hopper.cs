using System.Collections.Generic;
using System.Linq;
using Core;

namespace Chains
{
    public class ChainDefsBuilder
    {
        List<IChainDefBuilder> defs = new List<IChainDefBuilder>();
        public ChainDefBuilder<T> AddDef<T>(System.Func<IProvideBehavior, Chain<T>> path)
            where T : EventBase
        {
            var def = new ChainDefBuilder<T>(path);
            defs.Add(def);
            return def;
        }
        public IChainDef[] ToStatic()
        {
            return (IChainDef[])defs.Select(def => def.ToStatic());
        }
    }
}