using Chains;
using System.Collections.Generic;
using System.Numerics;

namespace Core
{
    public class Pushable : Behavior
    {
        public static List<string> s_indexSourceNameMap = new List<string>();

        public static int RegisterPushSource(string name, int defaultResValue = 1)
        {
            var sourceResFile = (ArrayFile)StatManager.s_defaultFS.GetFile("pushed/source_res");
            sourceResFile.content.Add(defaultResValue);

            s_indexSourceNameMap.Add(name);
            return s_indexSourceNameMap.Count - 1;
        }

        static Pushable()
        {
            Directory baseDir = StatManager.s_defaultFS.BaseDir;

            Directory pushDir = new Directory();
            File sourceResFile = new ArrayFile();
            File resFile = new Resistance
            {
                pierce = 1
            };

            baseDir.AddDirectory("pushed", pushDir);
            pushDir.AddFile("source_res", sourceResFile);
            pushDir.AddFile("res", resFile);

            RegisterPushSource("default", 1);
        }

        public class Resistance : File
        {
            public int pierce = 0;
        }

        public class Event : CommonEvent
        {
            public Entity entity;
            public Attacking.Push push;
            public Resistance resistance;
        }

        public class Params : ActivationParams
        {
            public Attacking.Push push;
        }

        Chain chain_checkPushed;
        Chain chain_bePushed;

        public Pushable(Entity entity, BehaviorConfig conf)
        {
            chain_checkPushed = entity.m_chains["pushed:check"];
            chain_bePushed = entity.m_chains["pushed:do"];
        }

        public override bool Activate(
            Entity actor,
            Action action,
            ActivationParams pars = null)
        {
            var ev = new Event
            {
                actor = actor,
                action = action,
                push = ((Params)pars).push
            };

            chain_checkPushed.Pass(ev);

            if (!ev.propagate)
                return false;

            chain_bePushed.Pass(ev);
            return true;
        }

        static void SetResistance(EventBase eventBase)
        {
            var ev = (Event)eventBase;
            ev.resistance = (Resistance)ev.actor.m_statManager.GetFile("pushed/res");
        }

        static void ResistSource(EventBase eventBase)
        {
            var ev = (Event)eventBase;
            var sourceRes = (ArrayFile)ev.actor.m_statManager.GetFile("pushed/source_res");
            if (sourceRes[ev.push.source] > ev.push.power)
            {
                ev.push.distance = 0;
            }
        }

        static void Armor(EventBase eventBase)
        {
            var ev = (Event)eventBase;

            if (ev.push.pierce <= ev.resistance.pierce)
            {
                ev.propagate = false;
            }
        }

        static void BePushed(EventBase eventBase)
        {
            var ev = (Event)eventBase;
            // TODO: set up properly
            var move = new Displaceable.Move();
            var pars = new Displaceable.Params { move = move };
            ev.entity.beh_Displaceable.Activate(ev.actor, ev.action, pars);
        }

        public static BehaviorFactory s_factory = new BehaviorFactory(
            typeof(Pushable), new ChainDefinition[]
            {
                new ChainDefinition
                {
                    name = "pushed:check",
                    handlers = new WeightedEventHandler[]
                    {
                        new WeightedEventHandler {
                            handlerFunction = SetResistance,
                            priority = (int)PRIORITY_RANKS.HIGH
                        },
                        new WeightedEventHandler {
                            handlerFunction = ResistSource,
                            priority = (int)PRIORITY_RANKS.LOW
                        },
                        new WeightedEventHandler {
                            handlerFunction = Armor,
                            priority = (int)PRIORITY_RANKS.LOW
                        }
                    }
                },
                new ChainDefinition
                {
                    name = "pushed:do",
                    handlers = new WeightedEventHandler[]
                    {
                        new WeightedEventHandler {
                            handlerFunction = BePushed
                        }
                    }
                }
            }
        );
    }
}