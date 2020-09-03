using System.Collections.Generic;
using System.Linq;
using Core.FS;
using Chains;

namespace Core.Behaviors
{
    public class Attacking : Behavior, IStandartActivateable
    {

        static void SetupStats()
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
        public static string s_checkChainName = "attack:check";
        public static string s_doChainName = "attack:do";

        public Attacking(Entity entity)
        {
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
        public bool Activate(Entity actor, Action action) => Activate(actor, action, null);
        public bool Activate(Entity actor, Action action, ActivationParams pars)
        {
            var ev = new Event
            {
                targets = ((Params)pars)?.targets,
                actor = actor,
                action = action
            };
            return CheckDoCycle<Event>(ev, s_checkChainName, s_doChainName);

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
                ev.targets = ev.actor.GetBehavior<Attacking>().GenerateTargets(ev);
            }
        }

        static void ApplyAttack(Event ev)
        {
            foreach (var target in ev.targets)
            {
                var action = ev.action.Copy();
                action.direction = target.direction;
                var attackable = target.entity.GetBehavior<Attackable>();
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
                Pushable pushable = target.entity.GetBehavior<Pushable>();
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


        public static ChainPath<Attacking, Event> check_chain;
        public static ChainPath<Attacking, Event> do_chain;

        static Attacking()
        {
            var builder = new ChainTemplateBuilder();

            var check = builder.AddTemplate<Event>(s_checkChainName);
            check_chain = new ChainPath<Attacking, Event>(s_checkChainName);
            check.AddHandler(SetBase, PRIORITY_RANKS.HIGH);
            check.AddHandler(GetTargets, PRIORITY_RANKS.MEDIUM);

            var _do = builder.AddTemplate<Event>(s_doChainName);
            do_chain = new ChainPath<Attacking, Event>(s_checkChainName);
            _do.AddHandler(ApplyAttack);
            _do.AddHandler(Utils.AddHistoryEvent(History.EventCode.attacking_do));

            BehaviorFactory<Attacking>.s_builder = builder;

            SetupStats();
        }
    }
}