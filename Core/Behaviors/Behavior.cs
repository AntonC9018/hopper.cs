using Chains;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Core.Behaviors
{

    public abstract class IBehaviorFactory
    {
        protected static IdGenerator s_idGenerator = new IdGenerator();
        public int id;

        public Dictionary<string, IChainTemplate> templates =
            new Dictionary<string, IChainTemplate>();
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
        new public static int id;
        static Action<BehaviorFactory<Beh>> s_setupFunction;
        public static int ClassSetup(Action<BehaviorFactory<Beh>> setupFunction)
        {
            id = IBehaviorFactory.s_idGenerator.GetNextId();
            s_setupFunction = setupFunction;
            return id;
        }
        public BehaviorFactory()
        {
            base.id = BehaviorFactory<Beh>.id;
            s_setupFunction(this);
        }
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