using Chains;
using System;

namespace Core
{
    public abstract class IChainDef
    {
        public string name;
        public IEvHandler[] handlers;

        public IChainDef() { }
        public IChainDef(string name, IEvHandler handler)
        {
            this.name = name;
            this.handlers = new IEvHandler[] { handler };
        }
        public abstract IChainTemplate CreateChainTemplate();
    }
    public class ChainDef<Event> : IChainDef where Event : EventBase
    {
        public ChainDef() : base() { }
        public ChainDef(string name, IEvHandler handler) : base(name, handler) { }
        public override IChainTemplate CreateChainTemplate()
        {
            return new ChainTemplate<Event>();
        }
    }

    public class ChainTemplateDefinition
    {
        public string name;
        IChainTemplate template;
        public IChainTemplate Template
        {
            get { return template.Clone(); }
            set { template = value; }
        }
    }

    public abstract class IBehaviorFactory
    {
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();
        public ChainTemplateDefinition[] m_chainTemplateDefinitions;

        public IBehaviorFactory(IChainDef[] chainDefinitions)
        {
            SetupTemplates(chainDefinitions);
        }

        public IBehaviorFactory(IChainDef chainDefinitions)
        {
            SetupTemplates(new IChainDef[] { chainDefinitions });
        }

        public IBehaviorFactory()
        {
            m_chainTemplateDefinitions = new ChainTemplateDefinition[0];
        }

        protected void SetupTemplates(IChainDef[] chainDefinitions)
        {
            m_chainTemplateDefinitions = new ChainTemplateDefinition[chainDefinitions.Length];
            for (int i = 0; i < chainDefinitions.Length; i++)
            {
                var chainDef = chainDefinitions[i];
                var template = chainDef.CreateChainTemplate();

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

        public abstract Behavior Instantiate(Entity entity, BehaviorConfig conf);
    }

    public class BehaviorFactory<Beh> : IBehaviorFactory
        where Beh : Behavior
    {
        public BehaviorFactory(IChainDef[] chainDefinitions)
            : base(chainDefinitions)
        { }

        public BehaviorFactory(IChainDef chainDefinition)
            : base(chainDefinition)
        { }

        public BehaviorFactory()
            : base()
        { }

        public override Behavior Instantiate(Entity entity, BehaviorConfig conf)
        {
            return (Behavior)Activator.CreateInstance(typeof(Beh), entity, conf);
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