using Chains;
using System.Collections.Generic;

namespace Core
{
    public class Acting : Behavior
    {
        public Chain chain_checkAction;
        public Chain chain_doAction;
        public Chain chain_failAction;
        public Chain chain_succeedAction;
        public Entity m_entity;
        public bool b_didAction = false;
        public bool b_doingAction = false;
        public bool b_didActionSucceed = false;
        public Action m_nextAction;

        public Acting(Entity entity, BehaviorParams pars)
        {
            m_entity = entity;
            chain_checkAction = entity.m_chains["action:check"];
            chain_doAction = entity.m_chains["action:do"];
            chain_failAction = entity.m_chains["action:fail"];
            chain_succeedAction = entity.m_chains["action:succeed"];
        }

        public class ActingEvent : CommonEvent
        {
            public bool success = false;
        }

        public override bool Activate(Entity actor, Action action)
        {
            var ev = new ActingEvent
            {
                actor = m_entity,
                action = action
            };

            if (m_nextAction == null)
            {
                b_didAction = true;
                ev.success = true;
                chain_succeedAction.Pass(ev);
                return true;
            }

            b_doingAction = true;
            chain_checkAction.Pass(ev);

            if (ev.propagate)
            {
                ev.success = true;
                chain_doAction.Pass(ev);
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

        // I do hate the amount of boilerplate here
        // Since we want to have just one copy of this factory per class
        // I don't want to bloat my instances with copies of this
        public static BehaviorFactory s_factory = new BehaviorFactory(
            typeof(Acting), new ChainDefinition[]
            {
                new ChainDefinition
                {
                    name = "action:check",
                    handlers = new WeightedEventHandler[]
                    {
                    }
                },
                new ChainDefinition
                {
                    name = "action:do",
                    handlers = new WeightedEventHandler[]
                    {
                    }
                },
                new ChainDefinition
                {
                    name = "action:fail",
                    handlers = new WeightedEventHandler[]
                    {
                    }
                },
                new ChainDefinition
                {
                    name = "action:succeed",
                    handlers = new WeightedEventHandler[]
                    {
                    }
                }
            }
        );
    }
}