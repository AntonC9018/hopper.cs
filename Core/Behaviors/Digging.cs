using Chains;
using System.Collections.Generic;
using Core.FS;
using Vector;

namespace Core.Behaviors
{
    public class Diggable : Behavior
    {
        public static int s_attackSource;
        static void SetupStats()
        {
            s_attackSource = Attackable.RegisterAttackSource("dig");

            Directory baseDir = StatManager.DefaultFS.BaseDir;
            StatFile digStatFile = new Dig
            {
                source = s_attackSource,
                power = 0,
                damage = 1,
                pierce = 10
            };
            baseDir.AddFile("dig", digStatFile);
        }

        public class Dig : StatFile
        {
            public int source;
            public int power;
            public int damage;
            public int pierce;

            public Attacking.Attack ToAttack()
            {
                return new Attacking.Attack
                {
                    source = s_attackSource,
                    power = power,
                    damage = damage,
                    pierce = pierce
                };
            }
        }

        public class Event : CommonEvent
        {
            public Dig dig;
            public List<Target> targets;
        }

        public class Params : ActivationParams
        {
        }

        public static string s_checkChainName = "dig:check";
        public static string s_doChainName = "dig:do";

        public bool Activate(Action action, Params pars)
        {
            var ev = new Event
            {
                actor = m_entity,
                action = action,
                // push = pars.push
            };
            return CheckDoCycle<Event>(ev, s_checkChainName, s_doChainName);
        }
        static void SetDig(Event ev)
        {
            ev.dig = (Dig)ev.actor.m_statManager.GetFile("dig");
        }

        static void GetTargets(Event ev)
        {
            // TODO: set targets via shovel
        }

        static void Attack(Event ev)
        {
            foreach (var target in ev.targets)
            {
                var pars = new Attackable.Params
                {
                    attack = ev.dig.ToAttack()
                };
                target.entity.GetBehavior<Attackable>().Activate(ev.action, pars);
            }
        }

        public static ChainPaths<Diggable, Event> Check;
        public static ChainPaths<Diggable, Event> Do;
        static Diggable()
        {
            var builder = new ChainTemplateBuilder();

            var check = builder.AddTemplate<Event>(s_checkChainName);
            Check = new ChainPaths<Diggable, Event>(s_checkChainName);
            check.AddHandler(SetDig, PRIORITY_RANKS.HIGH);
            check.AddHandler(GetTargets, PRIORITY_RANKS.HIGH);

            var _do = builder.AddTemplate<Event>(s_doChainName);
            Do = new ChainPaths<Diggable, Event>(s_checkChainName);
            _do.AddHandler(Attack);
            // _do.AddHandler(Utils.AddHistoryEvent(History.EventCode.pushed_do));

            BehaviorFactory<Diggable>.s_builder = builder;

            SetupStats();
        }
    }
}