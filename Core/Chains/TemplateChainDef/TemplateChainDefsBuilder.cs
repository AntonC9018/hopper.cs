using System.Collections.Generic;
using System.Linq;
using Hopper.Core.Behaviors;
using Hopper.Utils.Chains;

namespace Hopper.Core.Chains
{
    public class TemplateChainDefBuilder
    {
        List<I_TemplateChainDef_PartBuilder> defs = new List<I_TemplateChainDef_PartBuilder>();
        public TemplateChainDef_PartBuilder<T> AddDef<T>(IChainPaths<T> path)
            where T : EventBase
        {
            var def = new TemplateChainDef_PartBuilder<T>(path.TemplatePath, this);
            defs.Add(def);
            return def;
        }
        public ITemplateChainDef[] ToStatic()
        {
            return defs.Select(def => def.ToStatic()).ToArray<ITemplateChainDef>();
        }
    }
}