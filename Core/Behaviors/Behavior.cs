using Chains;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Core.Behaviors
{

    public abstract class BehaviorFactory
    {
        protected static IdGenerator s_idGenerator = new IdGenerator();
        public int id;
        public static Dictionary<System.Type, int> s_idMap =
            new Dictionary<System.Type, int>();
        protected Dictionary<ChainName, ChainTemplate> m_templates =
            new Dictionary<ChainName, ChainTemplate>();
        public abstract Behavior Instantiate(Entity entity, BehaviorConfig conf);
    }

    public class BehaviorFactory<Beh> : BehaviorFactory, IProvidesChainTemplate
        where Beh : Behavior, new()
    {
        public static ChainTemplateBuilder s_builder;

        static BehaviorFactory()
        {
            var type = typeof(Beh);
            var id = BehaviorFactory.s_idGenerator.GetNextId();
            s_idMap[type] = id;
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
        }
        public BehaviorFactory()
        {
            id = s_idMap[typeof(Beh)];
            m_templates = s_builder?.Templates;
        }

        public ChainTemplate<Event> GetTemplate<Event>(ChainName name) where Event : EventBase
        {
            return (ChainTemplate<Event>)m_templates[name];
        }

        public override Behavior Instantiate(Entity entity, BehaviorConfig conf)
        {
            Behavior behavior = new Beh();
            behavior.GenerateChains(m_templates);
            behavior.Init(entity, conf);
            return behavior;
        }

    }

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

    public interface IStandartActivateable
    {
        public bool Activate(Action action);
    }

    public abstract class BehaviorConfig
    {
    }

    public abstract class ActivationParams
    {
    }

}