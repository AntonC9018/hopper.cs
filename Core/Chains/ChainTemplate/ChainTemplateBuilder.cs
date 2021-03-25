using System.Collections.Generic;
using Hopper.Core.Components;
using Hopper.Utils.Chains;

namespace Hopper.Core.Chains
{
    public class ChainTemplateBuilder
    {
        protected Dictionary<ChainName, I_ChainTemplate_PartBuilder> m_templateBuilders
            = new Dictionary<ChainName, I_ChainTemplate_PartBuilder>();

        public Dictionary<ChainName, IChainTemplate> CreateTemplates()
        {
            var templates = new Dictionary<ChainName, IChainTemplate>();
            foreach (var key in m_templateBuilders.Keys)
            {
                templates.Add(key, m_templateBuilders[key].Template);
            }
            return templates;
        }

        public ChainTemplate_PartBuilder<T> AddTemplate<T>(ChainName name) where T : EventBase
        {
            var builder = new ChainTemplate_PartBuilder<T>(this);
            m_templateBuilders.Add(name, builder);
            return builder;
        }

        public ChainTemplate_PartBuilder<T> GetTemplate<T>(ChainName name) where T : EventBase
        {
            return (ChainTemplate_PartBuilder<T>)m_templateBuilders[name];
        }
    }
}