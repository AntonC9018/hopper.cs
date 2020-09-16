using Chains;
using System.Collections.Generic;
using Core.FS;
using Core;
using System.Linq;

namespace Core.Behaviors
{

    public class Statused : Behavior
    {
        public class Event : CommonEvent
        {
            public Attack attack;
            public MapFile resistance;
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
            Tick.Chain.ChainPath(entity.Behaviors).AddHandler(
                e =>
                {
                    foreach (var status in IdMap.Status.ActiveItems)
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
            ev.resistance = (MapFile)ev.actor.StatManager.GetFile("status_res");
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

            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(StatusSetup).TypeHandle);
        }

    }
}