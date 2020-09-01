using System.Collections.Generic;
using System.Linq;
using Core.FS;
using Chains;

namespace Core.Behaviors
{
    public class Attacking : IBehavior
    {

        static Attacking()
        {
            Directory baseDir = StatManager.s_defaultFS.BaseDir;

            StatFile attackStatFile = new Attack
            {
                source = 0,
                power = 1,
                damage = 1,
                pierce = 1
            };
            StatFile pushStatFile = new Push
            {
                source = 0,
                power = 1,
                distance = 1,
                pierce = 1
            };

            baseDir.AddFile("attack", attackStatFile);
            baseDir.AddFile("push", pushStatFile);
        }

        public class Attack : StatFile
        {
            public int source;
            public int power;
            public int damage;
            public int pierce;
        }

        public class Push : StatFile
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

        Chain<Event> chain_checkAttack;
        Chain<Event> chain_doAttack;

        public Attacking(Entity entity)
        {
            chain_checkAttack = (Chain<Event>)entity.m_chains["attack:check"];
            chain_doAttack = (Chain<Event>)entity.m_chains["attack:do"];
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

        public bool Activate(Entity actor, Action action, ActivationParams pars)
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

        static void SetBase(Event ev)
        {
            if (ev.attack == null)
            {
                ev.attack = (Attack)ev.actor.m_statManager.GetFile("attack");
            }
            if (ev.push == null)
            {
                ev.push = (Push)ev.actor.m_statManager.GetFile("push");
            }
        }

        static void GetTargets(Event ev)
        {
            if (ev.targets == null)
            {
                ev.targets = ev.actor.beh_Attacking.GenerateTargets(ev);
            }
        }

        static void ApplyAttack(Event ev)
        {
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

        static void ApplyPush(Event ev)
        {
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

        public static BehaviorFactory<Attacking> CreateFactory()
        {
            var fact = new BehaviorFactory<Attacking>();

            var check = fact.AddTemplate<Event>("attack:check");
            var setBaseHandler = new EvHandler<Event>(SetBase, PRIORITY_RANKS.HIGH);
            var getTargetsHandler = new EvHandler<Event>(GetTargets, PRIORITY_RANKS.MEDIUM);
            check.AddHandler(setBaseHandler);
            check.AddHandler(getTargetsHandler);

            var _do = fact.AddTemplate<Event>("attack:do");
            var applyAttackHandler = new EvHandler<Event>(ApplyAttack);
            var addEventHandler = new EvHandler<Event>(Utils.AddHistoryEvent(History.EventCode.attacking_do));
            _do.AddHandler(applyAttackHandler);
            _do.AddHandler(addEventHandler);

            return fact;
        }

        public static BehaviorFactory<Attacking> s_factory = CreateFactory();
    }
}