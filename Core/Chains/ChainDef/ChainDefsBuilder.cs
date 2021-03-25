using System.Collections.Generic;
using System.Linq;
using Hopper.Core;
using Hopper.Core.Components;
using Hopper.Utils.Chains;

namespace Hopper.Core.Chains
{
    public class ChainDefBuilder
    {
        List<I_ChainDef_PartBuilder> defs = new List<I_ChainDef_PartBuilder>();
        public ChainDef_PartBuilder<T> AddDef<T>(IChainPaths<T> path)
            where T : EventBase
        {
            var def = new ChainDef_PartBuilder<T>(path.ChainPath, this);
            defs.Add(def);
            return def;
        }
        public IChainDef[] ToStatic()
        {
            return defs.Select(def => def.ToStatic()).ToArray<IChainDef>();
        }
    }
}