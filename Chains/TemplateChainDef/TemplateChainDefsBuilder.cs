using System.Collections.Generic;
using System.Linq;
using Core;

namespace Chains
{
    public class TemplateChainDefBuilder
    {
        List<I_TCD_PartBuilder> defs = new List<I_TCD_PartBuilder>();
        public TCD_PartBuilder<T> AddDef<T>(BehaviorFactoryPath<T> path)
            where T : EventBase
        {
            var def = new TCD_PartBuilder<T>(path, this);
            defs.Add(def);
            return def;
        }
        public ITemplateChainDef[] ToStatic()
        {
            return defs.Select(def => def.ToStatic()).ToArray<ITemplateChainDef>();
        }
    }
}