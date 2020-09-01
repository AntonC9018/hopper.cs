using Chains;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Core.Behaviors
{
    public abstract class IChainDef
    {
        public string name;
        public IEvHandler[] handlers;
        public abstract IChainTemplate CreateChainTemplate();
    }
    public class ChainDef<Event> : IChainDef where Event : EventBase
    {
        public ChainDef() { }
        public ChainDef(string name, IEvHandler handler)
        {
            this.name = name;
            this.handlers = new IEvHandler[] { handler };
        }
        public ChainDef(string name, IEvHandler[] handlers)
        {
            this.name = name;
            this.handlers = handlers;
        }
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
        Dictionary<string, IChainTemplate> templates = new Dictionary<string, IChainTemplate>();
        public List<(string, IChainTemplate)> Templates
        {
            get
            {
                var arr = new List<(string, IChainTemplate)>();
                foreach (var (name, template) in templates)
                    arr.Add((name, template.Clone()));
                return arr;
            }
        }
        public abstract IBehavior Instantiate(Entity entity, BehaviorConfig conf);

        public ChainTemplate<T> AddTemplate<T>(string name) where T : EventBase
        {
            var ct = new ChainTemplate<T>();
            templates[name] = ct;
            return ct;
        }
    }

    public class BehaviorFactory<Beh> : IBehaviorFactory
        where Beh : IBehavior
    {
        public override IBehavior Instantiate(Entity entity, BehaviorConfig conf)
        {
            if (conf == null)
            {
                return (IBehavior)Activator.CreateInstance(typeof(Beh), entity);
            }
            return (IBehavior)Activator.CreateInstance(typeof(Beh), entity, conf);
        }
    }

    public interface IBehavior
    {
        // initialized chains
        // added handlers to existing chains (probably in form of behavior modifiers of some sort)

        // The question is whether I want to differentiate between the basic behaviors that simply add chains and handlers and e.g. the stats behavior
        // I probably don't and have them all inherit from this
        // abstract public Behavior(Entity entity) { }
        public bool Activate(Entity actor, Action action, ActivationParams pars = null)
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