using Chains;
using System.Collections.Generic;

namespace Core
{
    public class Acting : Behavior
    {
        public class Config : BehaviorConfig
        {
            public System.Func<Entity, Action> calculateAction;
            public System.Action<EventBase> doAction;
        }
        Chain<Event> chain_checkAction;
        Chain<Event> chain_failAction;
        Chain<Event> chain_succeedAction;
        Entity m_entity;
        public bool b_didAction = false;
        public bool b_doingAction = false;
        public bool b_didActionSucceed = false;
        public Action m_nextAction;
        System.Func<Entity, Action> conf_calculateAction;
        System.Action<Event> conf_doActionFunc;


        public Acting(Entity entity, BehaviorConfig conf)
        {
            m_entity = entity;
            chain_checkAction = (Chain<Event>)entity.m_chains["action:check"];
            chain_failAction = (Chain<Event>)entity.m_chains["action:fail"];
            chain_succeedAction = (Chain<Event>)entity.m_chains["action:succeed"];
            conf_calculateAction = ((Config)conf).calculateAction;
            conf_doActionFunc = ((Config)conf).doAction;

            entity.m_chains["tick"].AddHandler<CommonEvent>(e =>
            {
                b_didAction = false;
                b_doingAction = false;
                m_nextAction = null;
            });
        }

        public class Event : CommonEvent
        {
            public bool success = false;
        }

        public override bool Activate(
            Entity actor,
            Action action,
            ActivationParams pars)
        {
            throw new System.Exception("Acting decorator cannot be activated using the usual parameters: Entity actor, Action action, BehaviorActivationParams pars.");
        }

        public bool Activate()
        {
            var ev = new Event
            {
                actor = m_entity,
                action = m_nextAction
            };

            if (m_nextAction == null)
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
                conf_doActionFunc(ev);
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
            if (m_nextAction != null)
                return;

            if (conf_calculateAction != null)
                m_nextAction = conf_calculateAction(m_entity);
            else
            {
                var sequenced = m_entity.beh_Sequenced;
                m_nextAction = sequenced.CurrentAction;
            }
        }

        // I do hate the amount of boilerplate here
        // Since we want to have just one copy of this factory per class
        // I don't want to bloat my instances with copies of this
        public static BehaviorFactory<Acting> s_factory = new BehaviorFactory<Acting>(
            new IChainDef[]
            {
                new ChainDef<Event>
                {
                    name = "action:check",
                    handlers = new EvHandler<Event>[]
                    {
                    }
                },
                new ChainDef<Event>
                {
                    name = "action:fail",
                    handlers = new EvHandler<Event>[]
                    {
                    }
                },
                new ChainDef<Event>
                {
                    name = "action:succeed",
                    handlers = new EvHandler<Event>[]
                    {
                    }
                }
            }
        );
    }
}