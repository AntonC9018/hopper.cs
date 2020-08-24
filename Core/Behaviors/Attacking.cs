using System.Collections.Generic;
using System.Linq;
using Core.FS;
using Chains;

namespace Core
{
    public class Attacking : Behavior
    {

        static Attacking()
        {
            Directory baseDir = StatManager.s_defaultFS.BaseDir;

            File attackFile = new Attack
            {
                source = 0,
                power = 1,
                damage = 1,
                pierce = 1
            };
            File pushFile = new Push
            {
                source = 0,
                power = 1,
                distance = 1,
                pierce = 1
            };

            baseDir.AddFile("attack", attackFile);
            baseDir.AddFile("push", pushFile);
        }

        public class Attack : File
        {
            public int source;
            public int power;
            public int damage;
            public int pierce;
        }

        public class Push : File
        {
            public int source = 0;
            public int power = 1;
            public int distance = 1;
            public int pierce = 1;
        }

        public class Event : CommonEvent
        {
            public List<Target> targets;
            public Attack attack;
            public Push push;
        }

        public class Params : ActivationParams
        {
            public List<Target> targets;
        }

        Chain<CommonEvent> chain_checkAttack;
        Chain<CommonEvent> chain_doAttack;

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

        static void SetBase(CommonEvent commonEvent)
        {
            var ev = (Event)commonEvent;
            if (ev.attack == null)
            {
                ev.attack = (Attack)ev.actor.m_statManager.GetFile("attack");
            }
            if (ev.push == null)
            {
                ev.push = (Push)ev.actor.m_statManager.GetFile("push");
            }
        }

        static void GetTargets(CommonEvent commonEvent)
        {
            var ev = (Event)commonEvent;
            if (ev.targets == null)
            {
                ev.targets = ev.actor.beh_Attacking.GenerateTargets(ev);
            }
        }

        static void ApplyAttack(CommonEvent commonEvent)
        {
            var ev = (Event)commonEvent;
            foreach (var target in ev.targets)
            {
                var action = ev.action.Copy();
                action.direction = target.direction;
                var attackable = target.entity.beh_Attackable;
                var pars = new Attackable.Params
                {
                    attack = (Attack)ev.attack.Copy()
                };
                // let it throw if this has not been accounted for
                attackable.Activate(target.entity, action, pars);
            }
        }

        static void ApplyPush(CommonEvent commonEvent)
        {
            var ev = (Event)commonEvent;
            foreach (var target in ev.targets)
            {
                var action = ev.action.Copy();
                action.direction = target.direction;
                Pushable pushable = target.entity.beh_Pushable;
                if (pushable != null)
                {
                    var pars = new Pushable.Params
                    {
                        push = (Push)ev.push.Copy()
                    };
                    pushable.Activate(target.entity, action, pars);
                }
            }
        }

        public static BehaviorFactory s_factory = new BehaviorFactory(
            typeof(Attacking), new ChainDef<CommonEvent>[]
            {
                new ChainDef<CommonEvent>
                {
                    name = "attack:check",
                    handlers = new EvHandler<CommonEvent>[]
                    {
                        new EvHandler<CommonEvent> {
                            handlerFunction = SetBase,
                            priority = (int)PRIORITY_RANKS.HIGH
                        },
                        new EvHandler<CommonEvent> {
                            handlerFunction = GetTargets
                        }
                    }
                },
                new ChainDef<CommonEvent>
                {
                    name = "attack:do",
                    handlers = new EvHandler<CommonEvent>[]
                    {
                        new EvHandler<CommonEvent> {
                            handlerFunction = ApplyAttack
                        }
                    }
                }
            }
        );
    }
}