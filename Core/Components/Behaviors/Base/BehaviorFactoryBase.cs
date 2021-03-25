using System.Collections.Generic;
using Hopper.Core.Chains;
using Hopper.Utils.Chains;

namespace Hopper.Core.Components
{
    public class ConfiglessBehaviorFactory<T> : IBehaviorFactory<T>
        where T : Behavior, new()
    {
        public Dictionary<ChainName, IChainTemplate> templates;

        public ConfiglessBehaviorFactory(ChainTemplateBuilder builder)
        {
            this.templates = builder?.CreateTemplates();
        }

        public ChainTemplate<Event> GetTemplate<Event>(ChainName name) where Event : EventBase
        {
            return (ChainTemplate<Event>)templates[name];
        }

        public T Instantiate(Entity entity)
        {
            T behavior = new T();
            behavior._SetEntity(entity);
            if (templates != null)
            {
                behavior.GenerateChains(templates);
            }
            return behavior;
        }
    }
}