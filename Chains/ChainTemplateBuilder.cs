using System.Collections.Generic;
using Core.Behaviors;

namespace Chains
{
    public class ChainTemplateBuilder
    {
        protected Dictionary<ChainName, ChainTemplate> m_templates
            = new Dictionary<ChainName, ChainTemplate>();
        public Dictionary<ChainName, ChainTemplate> Templates
        {
            get
            {
                var templates = new Dictionary<ChainName, ChainTemplate>();
                foreach (var key in m_templates.Keys)
                {
                    templates.Add(key, m_templates[key].Clone());
                }
                return templates;
            }
        }
        public ChainTemplate<T> AddTemplate<T>(ChainName name) where T : EventBase
        {
            var template = new ChainTemplate<T>();
            m_templates.Add(name, template);
            return template;
        }

        public ChainTemplate<T> GetTemplate<T>(ChainName name) where T : EventBase
        {
            return (ChainTemplate<T>)m_templates[name];
        }
    }
}