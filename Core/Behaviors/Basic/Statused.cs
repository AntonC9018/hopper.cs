using Hopper.Utils.Chains;
using Hopper.Core.Stats.Basic;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Hopper.Core.Chains;
using Hopper.Core.Stats;

namespace Hopper.Core.Behaviors.Basic
{
    [DataContract]
    public class Statused : Behavior, IInitable
    {
        public class Event : EventBase
        {
            public Entity actor;
            public Attack attack;
            public DictFile resistance;
            public StatusParam[] statusParams;
        }

        public class Params
        {
            public StatusParam[] statusParams;
            public Params(StatusParam param)
            {
                statusParams = new StatusParam[] { param };
            }
        }

        [DataMember]
        private HashSet<IStatus> m_appliedStatuses = new HashSet<IStatus>();

        public void Init()
        {
            // this should be refactored into a retoucher
            Tick.Chain.ChainPath(m_entity.Behaviors).AddHandler(
                e => UpdateStatuses()
            );
        }

        // Returns a list of the statuses that were applied
        // public IEnumerable<IStatus> Activate(Params pars)
        public bool Activate(Params pars)
        {
            var ev = new Event
            {
                actor = m_entity,
                statusParams = pars.statusParams
            };
            GetChain<Event>(ChainName.Check).Pass(ev);
            AddStatuses(ev.statusParams);
            // foreach (var statusParam in ev.statusParams)
            // {
            //     yield return statusParam.status;
            // }
            return ev.statusParams.Length > 0;
        }

        private void UpdateStatuses()
        {
            foreach (var status in m_appliedStatuses.ToList())
            {
                status.Update(m_entity);
                if (status.IsApplied(m_entity) == false)
                {
                    m_appliedStatuses.Remove(status);
                }
            }
        }

        private void AddStatuses(StatusParam[] statusParams)
        {
            foreach (var par in statusParams)
            {
                m_appliedStatuses.Add(par.status);
            }
        }

        static void SetResistance(Event ev)
        {
            ev.resistance = ev.actor.Stats.GetLazy(Status.Source.Resistance.Path);
        }

        static void ResistSomeStatuses(Event ev)
        {
            ev.statusParams = ev.statusParams
                .Where(p => ev.resistance[p.status.SourceId] <= p.statusStat.power)
                .Where(p => p.statusStat.amount > 0)
                .ToArray();
        }

        public static readonly ChainPaths<Statused, Event> Check;

        public static readonly ChainTemplateBuilder DefaultBuilder;
        public static InitableBehaviorFactory<Statused> Preset =>
            new InitableBehaviorFactory<Statused>(DefaultBuilder);

        static Statused()
        {
            Check = new ChainPaths<Statused, Event>(ChainName.Check);

            DefaultBuilder = new ChainTemplateBuilder()

                .AddTemplate<Event>(ChainName.Check)
                .AddHandler(SetResistance, PriorityRank.High)
                .AddHandler(ResistSomeStatuses, PriorityRank.Low)

                // .AddHandler(Utils.AddHistoryEvent(History.UpdateCode.))
                .End();
        }

    }
}