using Chains;
using System.Collections.Generic;
using Core.FS;
using Core;
using System.Linq;

namespace Core.Behaviors
{
    public class StatusData
    {
        public Flavor flavor;
        public StatusFile statusStat;
        public int statusIndex;

        public StatusData(Flavor flavor, StatusFile statusStat, int statusIndex)
        {
            this.flavor = flavor;
            this.statusStat = statusStat;
            this.statusIndex = statusIndex;
        }
    }

    public class Statused : Behavior
    {
        public static List<string> s_indexStatusNameMap = new List<string>();
        public static List<IStatus> s_indexStatusMap = new List<IStatus>();

        public static int RegisterStatus(string name, IStatus status, int defaultResValue = 1)
        {
            var statusResFile = (ArrayFile)StatManager.DefaultFS.GetFile("status_res");
            statusResFile.content.Add(defaultResValue);

            s_indexStatusNameMap.Add(name);
            s_indexStatusMap.Add(status);
            return s_indexStatusNameMap.Count - 1;
        }

        static void SetupStats()
        {
            Directory baseDir = StatManager.DefaultFS.BaseDir;
            StatFile resFile = new ArrayFile();
            baseDir.AddFile("status_res", resFile);
        }

        public class Event : CommonEvent
        {
            public Attacking.Attack attack;
            public ArrayFile resistance;
            public StatusData[] statusData;
        }

        public class Params : ActivationParams
        {
            public StatusData[] statusData;
        }

        public override void Init(Entity entity, BehaviorConfig config)
        {
            m_entity = entity;

            // this should be refactored into a retoucher
            Tick.chain.ChainPath(entity).AddHandler(
                e =>
                {
                    foreach (var status in s_indexStatusMap)
                        status.Tick(e.actor);
                }
            );
        }

        public bool Activate(Action action, Params pars)
        {
            var ev = new Event
            {
                actor = m_entity,
                action = action,
                statusData = pars.statusData
            };
            return CheckDoCycle<Event>(ev);
        }

        static void SetResistance(Event ev)
        {
            ev.resistance = (ArrayFile)ev.actor.StatManager.GetFile("status_res");
        }

        static void ResistSomeStatuses(Event ev)
        {
            ev.statusData = ev.statusData
                .Where(d => ev.resistance[d.statusIndex] <= d.statusStat.power)
                .Where(d => d.statusStat.amount > 0)
                .ToArray();
        }

        static void Apply(Event ev)
        {
            foreach (var statusData in ev.statusData)
            {
                var status = s_indexStatusMap[statusData.statusIndex];
                statusData.flavor.amount = statusData.statusStat.amount;
                status.Apply(ev.actor, statusData.flavor);
            }
        }

        static Statused()
        {
            var builder = new ChainTemplateBuilder();

            var check = builder.AddTemplate<Event>(ChainName.Check);
            check.AddHandler(SetResistance, PriorityRanks.High);
            check.AddHandler(ResistSomeStatuses, PriorityRanks.Low);

            var _do = builder.AddTemplate<Event>(ChainName.Do);
            _do.AddHandler(Apply);
            _do.AddHandler(Utils.AddHistoryEvent(History.EventCode.attacking_do));

            BehaviorFactory<Statused>.s_builder = builder;

            SetupStats();
        }

    }
}