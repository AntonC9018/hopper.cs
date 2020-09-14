using System.Collections.Generic;
using System.Linq;
using Core;

namespace Chains
{
    public class ChainDefBuilder
    {
        List<I_CD_PartBuilder> defs = new List<I_CD_PartBuilder>();
        public CD_PartBuilder<T> AddDef<T>(BehaviorPath<T> path)
            where T : EventBase
        {
            var def = new CD_PartBuilder<T>(path, this);
            defs.Add(def);
            return def;
        }
        public IChainDef[] ToStatic()
        {
            return defs.Select(def => def.ToStatic()).ToArray<IChainDef>();
        }
    }
}