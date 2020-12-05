using System.Collections.Generic;
using System.Linq;
using Hopper.Core;
using Hopper.Core.Behaviors;
using Hopper.Utils.Chains;

namespace Hopper.Core.Chains
{
    public class ChainDefBuilder
    {
        List<I_CD_PartBuilder> defs = new List<I_CD_PartBuilder>();
        public CD_PartBuilder<T> AddDef<T>(IChainPaths<T> path)
            where T : EventBase
        {
            var def = new CD_PartBuilder<T>(path.ChainPath, this);
            defs.Add(def);
            return def;
        }
        public IChainDef[] ToStatic()
        {
            return defs.Select(def => def.ToStatic()).ToArray<IChainDef>();
        }
    }
}