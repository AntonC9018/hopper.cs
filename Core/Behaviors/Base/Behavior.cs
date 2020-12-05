using Hopper.Core.Chains;
using Hopper.Utils.Chains;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Hopper.Core.Behaviors
{
    [DataContract]
    public abstract class Behavior : IProvidesChain
    {
        protected Dictionary<ChainName, Chain> chains;
        protected Entity m_entity;

        public void GenerateChains(Dictionary<ChainName, IChainTemplate> templates)
        {
            if (templates == null)
                return;
            chains = new Dictionary<ChainName, Chain>();
            foreach (var kvp in templates)
            {
                chains[kvp.Key] = kvp.Value.Init();
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

        internal void _SetEntity(Entity entity)
        {
            m_entity = entity;
        }

        protected static void AssureRun(System.Type type)
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
        }
    }
}