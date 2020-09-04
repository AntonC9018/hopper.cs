using System.Collections.Generic;

namespace Chains
{
    public class ChainTemplateBuilder
    {
        protected Dictionary<string, ChainTemplate> m_templates
            = new Dictionary<string, ChainTemplate>();
        public Dictionary<string, ChainTemplate> Templates
        {
            get
            {
                var templates = new Dictionary<string, ChainTemplate>();
                foreach (var (key, val) in m_templates)
                {
                    templates.Add(key, val.Clone());
                }
                return templates;
            }
        }
        public ChainTemplate<T> AddTemplate<T>(string name) where T : EventBase
        {
            var template = new ChainTemplate<T>();
            m_templates.Add(name, template);
            return template;
        }
    }
}