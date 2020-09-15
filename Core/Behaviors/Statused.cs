using Chains;
using System.Collections.Generic;
using Core.FS;
using Core;
using System.Linq;

namespace Core.Behaviors
{
    public class StatusParam
    {
        public Flavor flavor;
        public StatusFile statusStat;
        public int statusId;

        public StatusParam(Flavor flavor, StatusFile statusStat, int statusId)
        {
            this.flavor = flavor;
            this.statusStat = statusStat;
            this.statusId = statusId;
        }
    }

    public class Statused : Behavior
    {
        public static void RegisterStatus(IStatus status, int defaultResValue = 1)
        {
            var statusResFile = (ArrayFile)StatManager.DefaultFS.GetFile("status_res");
            statusResFile.content.Add(defaultResValue);
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
            public StatusParam[] statusParams;
        }

        public class Params : ActivationParams
        {
            public StatusParam[] statusParams;
        }

        public override void Init(Entity entity, BehaviorConfig config)
        {
            m_entity = entity;

            // this should be refactored into a retoucher
            Tick.Chain.ChainPath(entity).AddHandler(
                e =>
                {
                    foreach (var status in IdMap.Status.AllItems)
                        ((IStatus)status).Tick(e.actor);
                }
            );
        }

        public bool Activate(Action action, Params pars)
        {
            var ev = new Event
            {
                actor = m_entity,
                action = action,
                statusParams = pars.statusParams
            };
            return CheckDoCycle<Event>(ev);
        }

        static void SetResistance(Event ev)
        {
            ev.resistance = (ArrayFile)ev.actor.StatManager.GetFile("status_res");
        }

        static void ResistSomeStatuses(Event ev)
        {
            ev.statusParams = ev.statusParams
                .Where(p => ev.resistance[p.statusId] <= p.statusStat.power)
                .Where(p => p.statusStat.amount > 0)
                .ToArray();
        }

        static void Apply(Event ev)
        {
            foreach (var statusData in ev.statusParams)
            {
                var status = IdMap.Status.Map(statusData.statusId);
                statusData.flavor.amount = statusData.statusStat.amount;
                status.Apply(ev.actor, statusData.flavor);
            }
        }
        public static ChainPaths<Statused, Event> Check;
        public static ChainPaths<Statused, Event> Do;
        static Statused()
        {
            Check = new ChainPaths<Statused, Event>(ChainName.Check);
            Do = new ChainPaths<Statused, Event>(ChainName.Check);

            var builder = new ChainTemplateBuilder()

                .AddTemplate<Event>(ChainName.Check)
                .AddHandler(SetResistance, PriorityRanks.High)
                .AddHandler(ResistSomeStatuses, PriorityRanks.Low)

                .AddTemplate<Event>(ChainName.Do)
                .AddHandler(Apply)
                .AddHandler(Utils.AddHistoryEvent(History.EventCode.attacking_do))

                .End();

            BehaviorFactory<Statused>.s_builder = builder;

            SetupStats();
        }

    }
}