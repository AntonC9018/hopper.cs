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
            Directory baseDir = StatManager.DefaultFS.BaseDir;

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
        public bool Activate(Action action) => Activate(action, null);
        public bool Activate(Action action, Params pars)
        {
            var ev = new Event
            {
                targets = pars?.targets,
                actor = m_entity,
                action = action
            };
            return CheckDoCycle<Event>(ev);
        }

        static void SetBase(Event ev)
        {
            if (ev.attack == null)
            {
                ev.attack = (Attack)ev.actor.StatManager.GetFile("attack");
            }
            if (ev.push == null)
            {
                ev.push = (Push)ev.actor.StatManager.GetFile("push");
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
                attackable.Activate(action, pars);
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
                    pushable.Activate(action, pars);
                }
            }
        }


        public static ChainPaths<Attacking, Event> Check;
        public static ChainPaths<Attacking, Event> Do;

        static Attacking()
        {
            var builder = new ChainTemplateBuilder();

            var check = builder.AddTemplate<Event>(ChainName.Check);
            Check = new ChainPaths<Attacking, Event>(ChainName.Check);
            check.AddHandler(SetBase, PriorityRanks.High);
            check.AddHandler(GetTargets, PriorityRanks.Medium);

            var _do = builder.AddTemplate<Event>(ChainName.Do);
            Do = new ChainPaths<Attacking, Event>(ChainName.Check);
            _do.AddHandler(ApplyAttack);
            _do.AddHandler(Utils.AddHistoryEvent(History.EventCode.attacking_do));

            BehaviorFactory<Attacking>.s_builder = builder;

            SetupStats();
        }
    }
}