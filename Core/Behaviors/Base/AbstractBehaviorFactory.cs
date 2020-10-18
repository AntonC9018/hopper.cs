using System.Collections.Generic;
using Chains;
using Utils;

namespace Core.Behaviors
{
    public abstract class BehaviorFactory
    {
        protected Dictionary<ChainName, IChainTemplate> m_templates =
            new Dictionary<ChainName, IChainTemplate>();
        public abstract Behavior Instantiate(Entity entity, BehaviorConfig conf);
    }
}