using Chains;
using System.Collections.Generic;

namespace Core.Behaviors
{
    public class Acting : Behavior
    {
        public class Config : BehaviorConfig
        {
            public System.Func<Entity, Action> CalculateAction;
            public System.Action<EventBase> DoAction;
        }
        // TODO: refactor into integers. Map integers to strings if needed
        public bool b_didAction = false;
        public bool b_doingAction = false;
        public bool b_didActionSucceed = false;
        public Action NextAction { get; set; }
        System.Func<Entity, Action> config_CalculateAction;
        System.Action<Event> config_DoActionFunc;

        public override void Init(Entity entity, BehaviorConfig conf)
        {
            var config = (Config)conf;
            m_entity = entity;
            config_CalculateAction = config.CalculateAction;
            config_DoActionFunc = config.DoAction;
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
                GetChain<Event>(ChainName.Success).Pass(ev);
                return true;
            }

            b_doingAction = true;
            GetChain<Event>(ChainName.Check).Pass(ev);

            if (ev.propagate)
            {
                ev.success = true;
                config_DoActionFunc(ev);
            }

            ev.propagate = true;

            if (ev.success)
                GetChain<Event>(ChainName.Success).Pass(ev);
            else
                GetChain<Event>(ChainName.Fail).Pass(ev);

            b_doingAction = false;
            b_didAction = false;
            b_didActionSucceed = ev.success;

            return ev.success;
        }

        public void CalculateNextAction()
        {
            if (NextAction != null)
                return;

            if (config_CalculateAction != null)
                NextAction = config_CalculateAction(m_entity);

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

            var check = builder.AddTemplate<Event>(ChainName.Check);
            Check = new ChainPaths<Acting, Event>(ChainName.Check);

            var fail = builder.AddTemplate<Event>(ChainName.Fail);
            fail_chain = new ChainPaths<Acting, Event>(ChainName.Fail);

            var succeed = builder.AddTemplate<Event>(ChainName.Success);
            succeed_chain = new ChainPaths<Acting, Event>(ChainName.Success);

            BehaviorFactory<Acting>.s_builder = builder;
        }

    }
}