using System.Collections.Generic;
using Chains;
using Utils;

namespace Core.Behaviors
{
    public abstract class BehaviorFactory
    {
        protected Dictionary<ChainName, ChainTemplate> m_templates =
            new Dictionary<ChainName, ChainTemplate>();
        public abstract Behavior Instantiate(Entity entity, BehaviorConfig conf);
    }
}