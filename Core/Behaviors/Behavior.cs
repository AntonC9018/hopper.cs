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
            new Dictionary<Type, int>();
        public Dictionary<string, IChainTemplate> templates =
            new Dictionary<string, IChainTemplate>();
        public abstract Behavior Instantiate(Entity entity, BehaviorConfig conf);

        public ChainTemplate<T> AddTemplate<T>(string name) where T : EventBase
        {
            var ct = new ChainTemplate<T>();
            templates[name] = ct;
            return ct;
        }
    }

    public class BehaviorFactory<Beh> : BehaviorFactory
        where Beh : Behavior
    {
        static Action<BehaviorFactory<Beh>> s_setupFunction;
        /*
            I want to define how the templates are defined in Behavior class definitions
            but use this in the factories. Since I can't reference static members in
            templated classes, I've gotta get a reference to that function that sets
            up the definitions into here. There are two ways to do this that I've figured out:
                 1. Set it up in static contructors of the Behavior subclasses. This 
                    will work only if the constructors are called before the class is used
                    for templating, which it isn't (templating doesn't force static contructors call)
                 2. Use reflection to get the static function. Reflection is bad and it forces
                    the classes to define the static method but it is better than the former solution,
                    since it also eliminates some of the boilerplate.
        */
        static BehaviorFactory()
        {
            var t = typeof(Beh);
            var id = BehaviorFactory.s_idGenerator.GetNextId();
            s_idMap[t] = id;

            var setupMethodField = t.GetMethod("SetupChainTemplates");

            if (setupMethodField == null)
            {
                s_setupFunction = f => { };
            }
            else
            {
                s_setupFunction = (System.Action<BehaviorFactory<Beh>>)Delegate
                    .CreateDelegate(
                        typeof(System.Action<BehaviorFactory<Beh>>),
                        setupMethodField
                    );
            }
        }
        public BehaviorFactory()
        {
            id = s_idMap[typeof(Beh)];
            s_setupFunction(this);
        }

        public override Behavior Instantiate(Entity entity, BehaviorConfig conf)
        {
            if (conf == null)
            {
                return (Behavior)Activator.CreateInstance(typeof(Beh), entity);
            }
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