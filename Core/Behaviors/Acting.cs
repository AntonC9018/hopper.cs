using Chains;
using System.Collections.Generic;

namespace Core.Behaviors
{
    public class Acting : Behavior
    {
        public class Config : BehaviorConfig
        {
            public System.Func<Entity, Action> calculateAction;
            public System.Action<EventBase> doAction;
        }
        // TODO: refactor into integers. Map integers to strings if needed
        public static string s_checkChainName = "action:check";
        public static string s_failChainName = "action:fail";
        public static string s_succeedChainName = "action:succeed";
        public bool b_didAction = false;
        public bool b_doingAction = false;
        public bool b_didActionSucceed = false;
        public Action NextAction { get; set; }
        Entity m_entity;
        System.Func<Entity, Action> config_calculateAction;
        System.Action<Event> config_doActionFunc;


        // this constructor shouldn't be enforced
        // TODO: refactor in an init method
        public Acting(Entity entity, Config conf)
        {
            m_entity = entity;
            config_calculateAction = conf.calculateAction;
            config_doActionFunc = conf.doAction;
            Tick.chain.ChainPath(entity).AddHandler(
                e =>
                {
                    b_didAction = false;
                    b_doingAction = false;
                    NextAction = null;
                }
            );
        }

        public class Event : CommonEvent
        {
            public bool success = false;
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
                GetChain<Event>(s_succeedChainName).Pass(ev);
                return true;
            }

            b_doingAction = true;
            GetChain<Event>(s_checkChainName).Pass(ev);

            if (ev.propagate)
            {
                ev.success = true;
                config_doActionFunc(ev);
            }

            ev.propagate = true;

            if (ev.success)
                GetChain<Event>(s_succeedChainName).Pass(ev);
            else
                GetChain<Event>(s_failChainName).Pass(ev);

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

            // TODO: this should be e.g. the default value of this function
            else
            {
                var sequenced = m_entity.GetBehavior<Sequential>();
                NextAction = sequenced.CurrentAction;
            }
        }


        // initialize here
        public static ChainPaths<Acting, Event> Check;
        public static ChainPaths<Acting, Event> fail_chain;
        public static ChainPaths<Acting, Event> succeed_chain;

        static Acting()
        {
            var builder = new ChainTemplateBuilder();

            var check = builder.AddTemplate<Event>(s_checkChainName);
            Check = new ChainPaths<Acting, Event>(s_checkChainName);

            var fail = builder.AddTemplate<Event>(s_failChainName);
            fail_chain = new ChainPaths<Acting, Event>(s_failChainName);

            var succeed = builder.AddTemplate<Event>(s_succeedChainName);
            succeed_chain = new ChainPaths<Acting, Event>(s_succeedChainName);

            BehaviorFactory<Acting>.s_builder = builder;
        }

    }
}