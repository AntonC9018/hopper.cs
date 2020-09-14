using Chains;
using System.Collections.Generic;
using Core.FS;
using Utils.Vector;

namespace Core.Behaviors
{
    public class Pushable : Behavior
    {
        public static List<string> s_indexSourceNameMap = new List<string>();

        public static int RegisterPushSource(string name, int defaultResValue = 1)
        {
            var sourceResFile = (ArrayFile)StatManager.DefaultFS.GetFile("pushed/source_res");
            sourceResFile.content.Add(defaultResValue);

            s_indexSourceNameMap.Add(name);
            return s_indexSourceNameMap.Count - 1;
        }

        static void SetupStats()
        {
            Directory baseDir = StatManager.DefaultFS.BaseDir;

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
            public Attacking.Push push;
            public Resistance resistance;
        }

        public class Params : ActivationParams
        {
            public Attacking.Push push;
        }

        public bool Activate(Action action, Params pars)
        {
            var ev = new Event
            {
                actor = m_entity,
                action = action,
                push = pars.push
            };
            return CheckDoCycle<Event>(ev);
        }

        static void SetResistance(Event ev)
        {
            ev.resistance = (Resistance)ev.actor.StatManager.GetFile("pushed/res");
        }

        static void ResistSource(Event ev)
        {
            var sourceRes = (ArrayFile)ev.actor.StatManager.GetFile("pushed/source_res");
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
            var pars = new Displaceable.Params(move);
            ev.actor.GetBehavior<Displaceable>().Activate(ev.action, pars);
        }

        public static ChainPaths<Pushable, Event> Check;
        public static ChainPaths<Pushable, Event> Do;
        static Pushable()
        {
            Check = new ChainPaths<Pushable, Event>(ChainName.Check);
            Do = new ChainPaths<Pushable, Event>(ChainName.Check);

            var builder = new ChainTemplateBuilder()

                .AddTemplate<Event>(ChainName.Check)
                .AddHandler(SetResistance, PriorityRanks.High)
                .AddHandler(ResistSource, PriorityRanks.High)
                .AddHandler(Armor, PriorityRanks.High)

                .AddTemplate<Event>(ChainName.Do)
                .AddHandler(BePushed)
                .AddHandler(Utils.AddHistoryEvent(History.EventCode.pushed_do))

                .End();

            BehaviorFactory<Pushable>.s_builder = builder;

            SetupStats();
        }
    }
}