using Chains;
using System.Collections.Generic;

namespace Core
{
    public class ChainDef<Event> where Event : EventBase
    {
        public string name;
        public EvHandler<Event>[] handlers;

        public ChainDef() { }
        public ChainDef(string name, EvHandler<Event> handler)
        {
            this.name = name;
            this.handlers = new EvHandler<Event>[] { handler };
        }
    }

    public class ChainTemplateDefinition
    {
        public string name;
        ChainTemplate<CommonEvent> template;
        public ChainTemplate<CommonEvent> Template
        {
            get { return template.Clone(); }
            set { template = value; }
        }
    }

    public class BehaviorFactory
    {
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();
        System.Type t_behaviorType;
        public ChainTemplateDefinition[] m_chainTemplateDefinitions;

        public BehaviorFactory(System.Type behaviorType, ChainDef<CommonEvent>[] chainDefinitions)
        {
            t_behaviorType = behaviorType;
            SetupTemplates(chainDefinitions);
        }

        public BehaviorFactory(System.Type behaviorClass, ChainDef<CommonEvent> chainDefinitions)
        {
            t_behaviorType = behaviorClass;
            SetupTemplates(new ChainDef<CommonEvent>[] { chainDefinitions });
        }

        public BehaviorFactory(System.Type behaviorClass)
        {
            t_behaviorType = behaviorClass;
            m_chainTemplateDefinitions = new ChainTemplateDefinition[0];
        }

        void SetupTemplates(ChainDef<CommonEvent>[] chainDefinitions)
        {
            m_chainTemplateDefinitions = new ChainTemplateDefinition[chainDefinitions.Length];
            for (int i = 0; i < chainDefinitions.Length; i++)
            {
                var chainDef = chainDefinitions[i];
                var template = new ChainTemplate<CommonEvent>();

                foreach (var func in chainDef.handlers)
                {
                    template.AddHandler(func);
                }

                m_chainTemplateDefinitions[i] = new ChainTemplateDefinition
                {
                    name = chainDef.name,
                    Template = template
                };
            }
        }

        public Behavior Instantiate(Entity entity, BehaviorConfig conf)
        {
            return (Behavior)System.Activator.CreateInstance(t_behaviorType, entity, conf);
        }
    }

    public abstract class Behavior
    {
        // initialized chains
        // added handlers to existing chains (probably in form of behavior modifiers of some sort)

        // The question is whether I want to differentiate between the basic behaviors that simply add chains and handlers and e.g. the stats behavior
        // I probably don't and have them all inherit from this
        // abstract public Behavior(Entity entity) { }
        public virtual bool Activate(
            Entity actor,
            Action action,
            ActivationParams conf = null)
        {
            return true;
        }

    }

    public abstract class BehaviorConfig
    {
    }

    public abstract class ActivationParams
    {
    }

}