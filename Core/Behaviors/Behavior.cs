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
        protected Dictionary<string, ChainTemplate> m_templates =
            new Dictionary<string, ChainTemplate>();
        public abstract Behavior Instantiate(Entity entity, BehaviorConfig conf);
    }

    public class BehaviorFactory<Beh> : BehaviorFactory, IProvidesChainTemplate
        where Beh : Behavior
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

        public ChainTemplate<Event> GetTemplate<Event>(string name) where Event : EventBase
        {
            return (ChainTemplate<Event>)m_templates[name];
        }

        public override Behavior Instantiate(Entity entity, BehaviorConfig conf)
        {
            Behavior behavior;
            if (conf == null)
            {
                behavior = (Behavior)Activator.CreateInstance(typeof(Beh), entity);
            }
            else
            {
                behavior = (Behavior)Activator.CreateInstance(typeof(Beh), entity, conf);
            }
            behavior.GenerateChains(m_templates);
            return behavior;
        }

    }

    public abstract class Behavior : IProvidesChain
    {
        protected Dictionary<string, Chain> chains;

        public void GenerateChains(Dictionary<string, ChainTemplate> templates)
        {
            if (templates == null)
                return;
            chains = new Dictionary<string, Chain>();
            foreach (var (key, template) in templates)
            {
                chains[key] = template.Init();
            }
        }

        public Chain<Event> GetChain<Event>(string name) where Event : EventBase
        {
            return (Chain<Event>)chains[name];
        }

        public bool CheckDoCycle<Event>(Event ev, string checkName, string doName) where Event : EventBase, new()
        {
            GetChain<Event>(checkName).Pass(ev);

            if (!ev.propagate)
                return false;

            GetChain<Event>(doName).Pass(ev);
            return true;
        }
    }

    public interface IStandartActivateable
    {
        public bool Activate(Entity entity, Action action);
    }

    public abstract class BehaviorConfig
    {
    }

    public abstract class ActivationParams
    {
    }

}