using System.Collections.Generic;
using System.Linq;
using Core;

namespace Chains
{
    public class TemplateChainDefsBuilder
    {
        List<ITemplateChainDefBuilder> defs = new List<ITemplateChainDefBuilder>();
        public TemplateChainDefBuilder<T> AddDef<T>(BehaviorFactoryPath<T> path)
            where T : EventBase
        {
            var def = new TemplateChainDefBuilder<T>(path, this);
            defs.Add(def);
            return def;
        }
        public ITemplateChainDef[] ToStatic()
        {
            return defs.Select(def => def.ToStatic()).ToArray<ITemplateChainDef>();
        }
    }
}