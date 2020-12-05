using System.Collections.Generic;
using System.Linq;
using Hopper.Core.Behaviors;
using Hopper.Utils.Chains;

namespace Hopper.Core.Chains
{
    public class TemplateChainDefBuilder
    {
        List<I_TCD_PartBuilder> defs = new List<I_TCD_PartBuilder>();
        public TCD_PartBuilder<T> AddDef<T>(IChainPaths<T> path)
            where T : EventBase
        {
            var def = new TCD_PartBuilder<T>(path.TemplatePath, this);
            defs.Add(def);
            return def;
        }
        public ITemplateChainDef[] ToStatic()
        {
            return defs.Select(def => def.ToStatic()).ToArray<ITemplateChainDef>();
        }
    }
}