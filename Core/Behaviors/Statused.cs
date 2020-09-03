using Chains;
using System.Collections.Generic;
using Core.FS;
using Core;
using System.Linq;

namespace Core.Behaviors
{
    public class Statused : Behavior
    {
        public static List<string> s_indexStatusNameMap = new List<string>();
        public static List<IStatus> s_indexStatusMap = new List<IStatus>();

        public static int RegisterStatus(string name, IStatus status, int defaultResValue = 1)
        {
            var statusResFile = (ArrayFile)StatManager.s_defaultFS.GetFile("status_res");
            statusResFile.content.Add(defaultResValue);

            s_indexStatusNameMap.Add(name);
            s_indexStatusMap.Add(status);
            return s_indexStatusNameMap.Count - 1;
        }

        static void SetupStats()
        {
            Directory baseDir = StatManager.s_defaultFS.BaseDir;
            StatFile resFile = new ArrayFile();
            baseDir.AddFile("status_res", resFile);
        }

        public class Event : CommonEvent
        {
            public Attacking.Attack attack;
            public ArrayFile resistance;
            public Flavor[] flavors;
        }

        public class Params : ActivationParams
        {
            public Flavor[] flavors;
        }

        public static string s_checkChainName = "statused:check";
        public static string s_doChainName = "statused:do";
        Entity m_entity;

        public Statused(Entity entity)
        {
            m_entity = entity;

            // this should be refactored into a retoucher
            entity
                .GetBehavior<Tick>()
                .GetChain<Tick.Event>(Tick.s_chainName)
                .AddHandler<Tick.Event>(
                    e =>
                    {
                        foreach (var status in s_indexStatusMap)
                        {
                            status.Tick(e.actor);
                        }
                    }
                );
        }

        public bool Activate(Entity actor, Action action, ActivationParams pars)
        {
            var ev = new Event
            {
                actor = m_entity,
                action = action,
                flavors = ((Params)pars).flavors
            };
            return CheckDoCycle<Event>(ev, s_checkChainName, s_doChainName);
        }

        static void SetResistance(Event ev)
        {
            ev.resistance = (ArrayFile)ev.actor.m_statManager.GetFile("status_res");
        }

        static void ResistSomeStatuses(Event ev)
        {
            ev.flavors = ev.flavors
                .Where(f => ev.resistance[f.source] <= f.power)
                .ToArray();
        }

        static void Apply(Event ev)
        {
            foreach (var f in ev.flavors)
            {
                var status = s_indexStatusMap[f.source];
                status.Apply(ev.actor, f);
            }
        }

        static Statused()
        {
            var builder = new ChainTemplateBuilder();

            var check = builder.AddTemplate<Event>(s_checkChainName);
            check.AddHandler(SetResistance, PRIORITY_RANKS.HIGH);
            check.AddHandler(ResistSomeStatuses, PRIORITY_RANKS.LOW);

            var _do = builder.AddTemplate<Event>(s_doChainName);
            _do.AddHandler(Apply);
            _do.AddHandler(Utils.AddHistoryEvent(History.EventCode.attacking_do));

            BehaviorFactory<Statused>.s_builder = builder;

            SetupStats();
        }

    }
}