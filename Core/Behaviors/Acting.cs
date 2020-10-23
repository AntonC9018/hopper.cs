using Chains;
using System.Runtime.Serialization;

namespace Core.Behaviors
{
    [DataContract]
    public class Acting : Behavior
    {
        public class Config : BehaviorConfig
        {
            public System.Func<Entity, Action> CalculateAction;
            public System.Action<Acting.Event> DoAction;

            public Config(System.Action<EventBase> doAction)
            {
                DoAction = doAction;
                CalculateAction = ent => ent.Behaviors.Get<Sequential>().CurrentAction;
            }

            public Config(System.Action<EventBase> doAction, System.Func<Entity, Action> calculateAction)
            {
                CalculateAction = calculateAction;
                DoAction = doAction;
            }
        }

        public bool b_didAction = false;
        public bool b_doingAction = false;
        public bool b_didActionSucceed = false;
        public Action NextAction { get; set; }
        private System.Func<Entity, Action> config_CalculateAction;
        private System.Action<Event> config_DoActionFunc;

        public override void Init(Entity entity, BehaviorConfig conf)
        {
            var config = (Config)conf;
            m_entity = entity;
            config_CalculateAction = config.CalculateAction;
            config_DoActionFunc = config.DoAction;
            Tick.Chain.ChainPath(entity.Behaviors).AddHandler(
                e =>
                {
                    b_didAction = false;
                    b_doingAction = false;
                    NextAction = null;
                },
                PriorityRanks.Low
            );
        }

        public class Event : StandartEvent
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
            b_didActionSucceed = ev.success;

            if (ev.success)
                GetChain<Event>(ChainName.Success).Pass(ev);
            else
                GetChain<Event>(ChainName.Fail).Pass(ev);

            b_didAction = true;
            b_doingAction = false;

            return ev.success;
        }

        public void CalculateNextAction()
        {
            if (NextAction == null && config_CalculateAction != null)
            {
                NextAction = config_CalculateAction(m_entity);
            }
        }


        // initialize here
        public static ChainPaths<Acting, Event> Check;
        public static ChainPaths<Acting, Event> Fail;
        public static ChainPaths<Acting, Event> Success;

        static Acting()
        {

            Check = new ChainPaths<Acting, Event>(ChainName.Check);
            Fail = new ChainPaths<Acting, Event>(ChainName.Fail);
            Success = new ChainPaths<Acting, Event>(ChainName.Success);

            var builder = new ChainTemplateBuilder()
                .AddTemplate<Event>(ChainName.Check)
                .AddTemplate<Event>(ChainName.Fail)
                .AddTemplate<Event>(ChainName.Success)
                .End();

            BehaviorFactory<Acting>.s_builder = builder;
        }

    }
}