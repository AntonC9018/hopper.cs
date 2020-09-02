using Chains;
using System.Collections.Generic;
using Core.FS;
using Core;
using System.Linq;

namespace Core.Behaviors
{
    public class Statused : IBehavior
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

        static Statused()
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
        Chain<Event> chain_checkStatused;
        Chain<Event> chain_beStatused;
        Entity m_entity;

        public Statused(Entity entity)
        {
            chain_checkStatused = (Chain<Event>)entity.m_chains[s_checkChainName];
            chain_beStatused = (Chain<Event>)entity.m_chains[s_doChainName];
            m_entity = entity;

            // this should be refactored into a retoucher
            entity.m_chains[Tick.s_chainName].AddHandler<Tick.Event>(e =>
            {
                foreach (var status in s_indexStatusMap)
                {
                    status.Tick(e.actor);
                }
            });
        }

        public bool Activate(Entity actor, Action action, ActivationParams pars)
        {
            var ev = new Event
            {
                actor = m_entity,
                action = action,
                flavors = ((Params)pars).flavors
            };
            chain_checkStatused.Pass(ev);

            if (!ev.propagate)
                return false;

            chain_beStatused.Pass(ev);
            return true;
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

        public static void SetupChainTemplates(BehaviorFactory<Statused> fact)
        {
            var check = fact.AddTemplate<Event>(s_checkChainName);
            var setBaseHandler = new EvHandler<Event>(SetResistance, PRIORITY_RANKS.HIGH);
            var getTargetsHandler = new EvHandler<Event>(ResistSomeStatuses, PRIORITY_RANKS.LOW);
            check.AddHandler(setBaseHandler);
            check.AddHandler(getTargetsHandler);

            var _do = fact.AddTemplate<Event>(s_doChainName);
            var applyHandler = new EvHandler<Event>(Apply);
            var addEventHandler = new EvHandler<Event>(Utils.AddHistoryEvent(History.EventCode.attacking_do));
            _do.AddHandler(applyHandler);
            _do.AddHandler(addEventHandler);
        }

        public static int id = BehaviorFactory<Statused>.ClassSetup(SetupChainTemplates);

    }
}