using Chains;
using System.Collections.Generic;
using System.Numerics;

namespace Core
{
    public class Pushable : Behavior
    {

        public class Resistance
        {
            public int level = 0;
        }

        public class Push
        {
            public int source = 0;
            public int power = 1;

            // TODO
            public Push Copy()
            {
                return new Push();
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

        static void SetResistance(EventBase e)
        {
            var ev = (Event)e;
            // TODO:
            ev.resistance = new Resistance();
        }

        static void ResistSource(EventBase e)
        {
            // TODO
        }

        static void Armor(EventBase e)
        {
            var ev = (Event)e;

            if (ev.push.power <= ev.resistance.level)
            {
                e.propagate = false;
            }
        }

        static void BePushed(EventBase e)
        {
            var ev = (Event)e;
            System.Console.WriteLine($"Pushed by {ev.push.power}");
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