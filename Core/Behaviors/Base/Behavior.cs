using Chains;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Core.Behaviors
{
    public abstract class Behavior : IProvidesChain
    {
        protected Dictionary<ChainName, Chain> chains;
        protected Entity m_entity;

        public void GenerateChains(Dictionary<ChainName, ChainTemplate> templates)
        {
            if (templates == null)
                return;
            chains = new Dictionary<ChainName, Chain>();
            foreach (var (key, template) in templates)
            {
                chains[key] = template.Init();
            }
        }

        public Chain<Event> GetChain<Event>(ChainName name) where Event : EventBase
        {
            return (Chain<Event>)chains[name];
        }

        public bool CheckDoCycle<Event>(Event ev)
            where Event : EventBase, new()
        {
            GetChain<Event>(ChainName.Check).Pass(ev);

            if (!ev.propagate)
                return false;

            GetChain<Event>(ChainName.Do).Pass(ev);
            return true;
        }

        public virtual void Init(Entity entity, BehaviorConfig config)
        {
            m_entity = entity;
        }
    }


    public abstract class BehaviorConfig
    {
    }

    public abstract class ActivationParams
    {
    }
}