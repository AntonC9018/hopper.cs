using Chains;
using System.Collections.Generic;

namespace Core
{
    public struct ChainDefinition
    {
        public string name;
        public WeightedEventHandler[] handlers;
    }

    public struct ChainTemplateDefinition
    {
        public string name;
        public ChainTemplate template
        {
            get { return template.Clone(); }
            set { template = value; }
        }
    }

    public class BehaviorFactory
    {
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();

        public BehaviorFactory(
            System.Type behaviorType,
            ChainDefinition[] chainDefinitions)
        {
            t_behaviorType = behaviorType;
            m_chainTemplateDefinitions = new ChainTemplateDefinition[chainDefinitions.Length];

            for (int i = 0; i < chainDefinitions.Length; i++)
            {
                var chainDef = chainDefinitions[i];
                var template = new ChainTemplate();

                foreach (var func in chainDef.handlers)
                {
                    template.AddHandler(func);
                }

                m_chainTemplateDefinitions[i] = new ChainTemplateDefinition
                {
                    name = chainDef.name,
                    template = template
                };
            }
            System.Console.WriteLine("Hello from behaviour factory constructor");
        }
        // public BehaviorFactory(System.Type behaviorClass, ChainDefinition[] chainDefinitions)
        // {
        //     t_behaviorType = behaviorClass;
        //     m_chainDefinitions = chainDefinitions;
        // }

        // public BehaviorFactory(System.Type behaviorClass, ChainDefinition chainDefinition)
        // {
        //     t_behaviorType = behaviorClass;
        //     m_chainDefinitions = new ChainDefinition[] { chainDefinition };
        // }

        System.Type t_behaviorType;
        public ChainTemplateDefinition[] m_chainTemplateDefinitions;

        public Behavior Instantiate(Entity entity, BehaviorParams pars)
        {
            return (Behavior)System.Activator.CreateInstance(t_behaviorType, entity, pars);
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
            BehaviorActivationParams pars)
        {
            return true;
        }

    }

    public abstract class BehaviorParams
    {
    }

    public abstract class BehaviorActivationParams
    {
    }




}