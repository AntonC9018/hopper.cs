using System.Collections.Generic;
using Hopper.Core.Behaviors;

namespace Chains
{
    public class ChainTemplateBuilder
    {
        protected Dictionary<ChainName, I_CT_PartBuilder> m_templateBuilders
            = new Dictionary<ChainName, I_CT_PartBuilder>();
        public Dictionary<ChainName, IChainTemplate> Templates
        {
            get
            {
                var templates = new Dictionary<ChainName, IChainTemplate>();
                foreach (var key in m_templateBuilders.Keys)
                {
                    templates.Add(key, m_templateBuilders[key].Template);
                }
                return templates;
            }
        }
        public CT_PartBuilder<T> AddTemplate<T>(ChainName name) where T : EventBase
        {
            var builder = new CT_PartBuilder<T>(this);
            m_templateBuilders.Add(name, builder);
            return builder;
        }

        public CT_PartBuilder<T> GetTemplate<T>(ChainName name) where T : EventBase
        {
            return (CT_PartBuilder<T>)m_templateBuilders[name];
        }
    }
}