using System.Collections.Generic;
using Chains;

namespace Core
{
    public class Attacking : Behavior
    {

        public class Event : CommonEvent
        {
            public List<Target> targets;
            public Attackable.Attack attack;
            public Pushable.Push push;
        }

        public class Params : ActivationParams
        {
            public List<Target> targets;
        }

        Chain chain_checkAttack;
        Chain chain_doAttack;

        public Attacking(Entity entity, BehaviorConfig conf)
        {
            chain_checkAttack = entity.m_chains["attack:check"];
            chain_doAttack = entity.m_chains["attack:do"];
        }

        public List<Target> GenerateTargets(Event e)
        {
            var entity = e.actor
                .GetCellRelative(e.action.direction)
                .GetEntityFromLayer(Layer.REAL);

            if (entity == null)
            {
                return new List<Target>();
            }
            // for now, no weapon
            return new List<Target>
            {
                new Target { entity = entity, direction = e.action.direction }
            };
        }

        public override bool Activate(Entity actor, Action action, ActivationParams pars)
        {
            var ev = new Event
            {
                targets = ((Params)pars)?.targets,
                actor = actor,
                action = action
            };
            chain_checkAttack.Pass(ev);

            if (!ev.propagate)
                return false;

            chain_doAttack.Pass(ev);

            return true;
        }

        static void SetBase(EventBase eventBase)
        {
            var ev = (Event)eventBase;
            if (ev.attack == null)
            {
                // TODO: set stats for attack
                ev.attack = new Attackable.Attack();
            }
            if (ev.push == null)
            {
                ev.push = new Pushable.Push();
            }
        }

        static void GetTargets(EventBase eventBase)
        {
            var ev = (Event)eventBase;
            if (ev.targets == null)
            {
                ev.targets = ev.actor.beh_Attacking.GenerateTargets(ev);
            }
        }

        static void ApplyAttack(EventBase eventBase)
        {
            var ev = (Event)eventBase;
            foreach (var target in ev.targets)
            {
                var action = ev.action.Copy();
                action.direction = target.direction;
                var attackable = target.entity.beh_Attackable;
                var pars = new Attackable.Params
                {
                    attack = ev.attack.Copy()
                };
                // let it throw if this has not been accounted for
                attackable.Activate(target.entity, action, pars);
            }
        }

        static void ApplyPush(EventBase eventBase)
        {
            var ev = (Event)eventBase;
            foreach (var target in ev.targets)
            {
                var action = ev.action.Copy();
                action.direction = target.direction;
                Pushable pushable = target.entity.beh_Pushable;
                if (pushable != null)
                {
                    var pars = new Pushable.Params
                    {
                        push = ev.push.Copy()
                    };
                    pushable.Activate(target.entity, action, pars);
                }
            }
        }

        public static BehaviorFactory s_factory = new BehaviorFactory(
            typeof(Attacking), new ChainDefinition[]
            {
                new ChainDefinition
                {
                    name = "attack:check",
                    handlers = new WeightedEventHandler[]
                    {
                        new WeightedEventHandler {
                            handlerFunction = SetBase,
                            priority = (int)PRIORITY_RANKS.HIGH
                        },
                        new WeightedEventHandler {
                            handlerFunction = GetTargets
                        }
                    }
                },
                new ChainDefinition
                {
                    name = "attack:do",
                    handlers = new WeightedEventHandler[]
                    {
                        new WeightedEventHandler {
                            handlerFunction = ApplyAttack
                        }
                    }
                }
            }
        );
    }
}