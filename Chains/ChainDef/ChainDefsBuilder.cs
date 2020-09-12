using System.Collections.Generic;
using System.Linq;
using Core;

namespace Chains
{
    public class ChainDefsBuilder
    {
        List<IChainDefBuilder> defs = new List<IChainDefBuilder>();
        public ChainDefBuilder<T> AddDef<T>(BehaviorPath<T> path)
            where T : EventBase
        {
            var def = new ChainDefBuilder<T>(path, this);
            defs.Add(def);
            return def;
        }
        public IChainDef[] ToStatic()
        {
            return defs.Select(def => def.ToStatic()).ToArray<IChainDef>();
        }
    }
}