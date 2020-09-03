using Chains;
using System.Collections.Generic;
using Core.FS;
using Vector;

namespace Core.Behaviors
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

        static void SetupStats()
        {
            Directory baseDir = StatManager.s_defaultFS.BaseDir;

            Directory pushDir = new Directory();
            StatFile sourceResFile = new ArrayFile();
            StatFile resFile = new Resistance
            {
                pierce = 1
            };

            baseDir.AddDirectory("pushed", pushDir);
            pushDir.AddFile("source_res", sourceResFile);
            pushDir.AddFile("res", resFile);

            RegisterPushSource("default", 1);
        }

        public class Resistance : StatFile
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

        public static string s_checkChainName = "pushed:check";
        public static string s_doChainName = "pushed:do";

        public Pushable(Entity entity)
        {
        }

        public bool Activate(Entity actor, Action action, ActivationParams pars = null)
        {
            var ev = new Event
            {
                actor = actor,
                action = action,
                push = ((Params)pars).push
            };
            return CheckDoCycle<Event>(ev, s_checkChainName, s_doChainName);

        }

        static void SetResistance(Event ev)
        {
            ev.resistance = (Resistance)ev.actor.m_statManager.GetFile("pushed/res");
        }

        static void ResistSource(Event ev)
        {
            var sourceRes = (ArrayFile)ev.actor.m_statManager.GetFile("pushed/source_res");
            if (sourceRes[ev.push.source] > ev.push.power)
            {
                ev.push.distance = 0;
            }
        }

        static void Armor(Event ev)
        {
            if (ev.push.pierce <= ev.resistance.pierce)
            {
                ev.propagate = false;
            }
        }

        static void BePushed(Event ev)
        {
            // TODO: set up properly
            var move = new Displaceable.Move();
            var pars = new Displaceable.Params { move = move };
            ev.entity.GetBehavior<Displaceable>().Activate(ev.actor, ev.action, pars);
        }

        public static ChainPath<Pushable, Event> check_chain;
        public static ChainPath<Pushable, Event> do_chain;
        static Pushable()
        {
            var builder = new ChainTemplateBuilder();

            var check = builder.AddTemplate<Event>(s_checkChainName);
            check_chain = new ChainPath<Pushable, Event>(s_checkChainName);
            check.AddHandler(SetResistance, PRIORITY_RANKS.HIGH);
            check.AddHandler(ResistSource, PRIORITY_RANKS.HIGH);
            check.AddHandler(Armor, PRIORITY_RANKS.HIGH);

            var _do = builder.AddTemplate<Event>(s_doChainName);
            do_chain = new ChainPath<Pushable, Event>(s_checkChainName);
            _do.AddHandler(BePushed);
            _do.AddHandler(Utils.AddHistoryEvent(History.EventCode.pushed_do));

            BehaviorFactory<Pushable>.s_builder = builder;

            SetupStats();
        }
    }
}