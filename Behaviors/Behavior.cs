using Chains;
using System.Collections.Generic;

namespace Core
{
    public struct ChainDefinition
    {
        public string name;
        public List<WeightedEventHandler> handlerFuncs;
    }

    public class BehaviorFactory
    {
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();

        public BehaviorFactory()
        {
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

        public System.Type t_behaviorType;
        public ChainDefinition[] m_chainDefinitions;
        public System.Collections.Generic.IEnumerator<(string, ChainTemplate)> GetEnumerator()
        {
            foreach (var chainDef in m_chainDefinitions)
            {
                var template = new ChainTemplate();
                foreach (var func in chainDef.handlerFuncs)
                {
                    template.AddHandler(func);
                }
                yield return (chainDef.name, template);
            }
        }
        public Behavior Instantiate(Entity entity)
        {
            return (Behavior)System.Activator.CreateInstance(t_behaviorType, entity);
        }
    }

    public interface Behavior
    {
        // initialized chains
        // added handlers to existing chains (probably in form of behavior modifiers of some sort)

        // The question is whether I want to differentiate between the basic behaviors that simply add chains and handlers and e.g. the stats behavior
        // I probably don't and have them all inherit from this
        // abstract public Behavior(Entity entity) { }
        public void Activate();

    }




}