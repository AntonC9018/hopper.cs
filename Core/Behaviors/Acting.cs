using Chains;
using System.Collections.Generic;

namespace Core.Behaviors
{
    public class Acting : IBehavior
    {
        public class Config : BehaviorConfig
        {
            public System.Func<Entity, Action> calculateAction;
            public System.Action<EventBase> doAction;
        }
        public static string s_checkChainName = "action:check";
        public static string s_failChainName = "action:fail";
        public static string s_succeedChainName = "action:succeed";
        public bool b_didAction = false;
        public bool b_doingAction = false;
        public bool b_didActionSucceed = false;
        public Action NextAction { get; set; }
        Chain<Event> chain_checkAction;
        Chain<Event> chain_failAction;
        Chain<Event> chain_succeedAction;
        Entity m_entity;
        System.Func<Entity, Action> config_calculateAction;
        System.Action<Event> config_doActionFunc;


        public Acting(Entity entity, Config conf)
        {
            m_entity = entity;
            chain_checkAction = (Chain<Event>)entity.m_chains[s_checkChainName];
            chain_failAction = (Chain<Event>)entity.m_chains[s_failChainName];
            chain_succeedAction = (Chain<Event>)entity.m_chains[s_succeedChainName];
            config_calculateAction = conf.calculateAction;
            config_doActionFunc = conf.doAction;

            entity.m_chains[Tick.s_chainName].AddHandler<Tick.Event>(e =>
            {
                b_didAction = false;
                b_doingAction = false;
                NextAction = null;
            });
        }

        public class Event : CommonEvent
        {
            public bool success = false;
        }

        public bool Activate(Entity actor, Action action, ActivationParams pars)
        {
            throw new System.Exception("Acting decorator cannot be activated using the usual parameters: Entity actor, Action action, BehaviorActivationParams pars.");
        }

        public bool Activate()
        {
            var ev = new Event
            {
                actor = m_entity,
                action = NextAction
            };

            if (NextAction == null)
            {
                b_didAction = true;
                b_didActionSucceed = true;
                chain_succeedAction.Pass(ev);
                return true;
            }

            b_doingAction = true;
            chain_checkAction.Pass(ev);

            if (ev.propagate)
            {
                ev.success = true;
                config_doActionFunc(ev);
            }

            ev.propagate = true;

            if (ev.success)
                chain_succeedAction.Pass(ev);
            else
                chain_failAction.Pass(ev);

            b_doingAction = false;
            b_didAction = false;
            b_didActionSucceed = ev.success;

            return ev.success;
        }

        public void CalculateNextAction()
        {
            if (NextAction != null)
                return;

            if (config_calculateAction != null)
                NextAction = config_calculateAction(m_entity);
            else
            {
                var sequenced = m_entity.beh_Sequenced;
                NextAction = sequenced.CurrentAction;
            }
        }

        public static void SetupChainTemplates(BehaviorFactory<Acting> fact)
        {
            var check = fact.AddTemplate<Event>(s_checkChainName);
            var fail = fact.AddTemplate<Event>(s_failChainName);
            var succeed = fact.AddTemplate<Event>(s_succeedChainName);
        }

        public static int id = BehaviorFactory<Acting>.ClassSetup(SetupChainTemplates);
    }
}