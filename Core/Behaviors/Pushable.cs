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
            var pushDir = StatManager.s_defaultStatsDir.directories["push"];
            var sourceResDir = pushDir.directories["source_res"];
            pushDir.files.Add(name, defaultResValue);
            s_indexSourceNameMap.Add(name);
            return s_indexSourceNameMap.Count - 1;
        }

        static Pushable()
        {
            var baseDir = StatManager.s_defaultStatsDir;

            var pushDir = new Directory<int>();
            pushDir.files = new Dictionary<string, int>
            {
                { "source", 0 },
                { "power", 1 },
                { "distance", 1 },
                { "pierce", 1 }
            };

            var sourceResDir = new Directory<int>();
            sourceResDir.files = new Dictionary<string, int>
            {
                { "basic", 1 }
            };

            var resDir = new Directory<int>();
            resDir.files = new Dictionary<string, int>
            {
                { "pierce", 0 }
            };

            baseDir.directories.Add("push", pushDir);
            pushDir.directories.Add("source_res", sourceResDir);
            pushDir.directories.Add("res", resDir);
        }

        public class Resistance
        {
            public int pierce = 0;

            public static implicit operator Resistance(Dictionary<string, int> operand)
            {
                return new Resistance
                {
                    pierce = operand["pierce"]
                };
            }
        }

        public class Push
        {
            public int source = 0;
            public int power = 1;
            public int distance = 1;
            public int pierce = 1;

            // TODO
            public Push Copy()
            {
                return new Push();
            }

            public static implicit operator Push(Dictionary<string, int> operand)
            {
                return new Push
                {
                    source = operand["source"],
                    power = operand["power"],
                    pierce = operand["pierce"],
                    distance = operand["distance"]
                };
            }
        }

        public class Event : CommonEvent
        {
            public Entity entity;
            public Push push;
            public Resistance resistance;
        }

        public class Params : ActivationParams
        {
            public Push push;
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
            ev.resistance = ev.actor.m_statManager.GetStats("push/res");
        }

        static void ResistSource(EventBase eventBase)
        {
            var ev = (Event)eventBase;
            var sourceName = s_indexSourceNameMap[ev.push.source];
            var sourceRes = ev.actor.m_statManager.GetStats("push/source_res");
            if (sourceRes[sourceName] > ev.push.power)
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