using Hopper.Core.Chains;
using Hopper.Utils.Chains;
using System.Runtime.Serialization;

namespace Hopper.Core.Behaviors.Basic
{
    [DataContract]
    public class Acting : Behavior, IInitable<Acting.Config>
    {
        public class Config
        {
            public System.Func<Entity, Action> CalculateAction;
            public System.Action<Acting.Event> DoAction;

            public Config(System.Action<Acting.Event> doAction)
            {
                DoAction = doAction;
                CalculateAction = ent => ent.Behaviors.Get<Sequential>().CurrentAction;
            }

            public Config(System.Action<Acting.Event> doAction, System.Func<Entity, Action> calculateAction)
            {
                CalculateAction = calculateAction;
                DoAction = doAction;
            }
        }

        public bool DidAction { get; private set; }
        public bool DoingAction { get; private set; }
        public bool DidActionSucceed { get; private set; }
        public Action NextAction { get; set; }
        private Config m_config;

        public void Init(Config config)
        {
            m_config = config;
            Tick.Chain.ChainPath(m_entity.Behaviors).AddHandler(
                e =>
                {
                    DidAction = false;
                    DoingAction = false;
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
                DidAction = true;
                DidActionSucceed = true;
                GetChain<Event>(ChainName.Success).Pass(ev);
                return true;
            }

            DoingAction = true;
            GetChain<Event>(ChainName.Check).Pass(ev);

            if (ev.propagate)
            {
                ev.success = true;
                m_config.DoAction(ev);
            }

            ev.propagate = true;
            DidActionSucceed = ev.success;

            if (ev.success)
                GetChain<Event>(ChainName.Success).Pass(ev);
            else
                GetChain<Event>(ChainName.Fail).Pass(ev);

            DidAction = true;
            DoingAction = false;

            return ev.success;
        }

        public void CalculateNextAction()
        {
            if (NextAction == null && m_config.CalculateAction != null)
            {
                NextAction = m_config.CalculateAction(m_entity);
            }
        }

        public static readonly ChainPaths<Acting, Event> Check;
        public static readonly ChainPaths<Acting, Event> Fail;
        public static readonly ChainPaths<Acting, Event> Success;

        public static readonly ChainTemplateBuilder DefaultBuilder;
        public static ConfigurableBehaviorFactory<Acting, Config> Preset(Config config) =>
            new ConfigurableBehaviorFactory<Acting, Config>(DefaultBuilder, config);

        static Acting()
        {

            Check = new ChainPaths<Acting, Event>(ChainName.Check);
            Fail = new ChainPaths<Acting, Event>(ChainName.Fail);
            Success = new ChainPaths<Acting, Event>(ChainName.Success);

            DefaultBuilder = new ChainTemplateBuilder()
                .AddTemplate<Event>(ChainName.Check)
                .AddTemplate<Event>(ChainName.Fail)
                .AddTemplate<Event>(ChainName.Success)
                .End();
        }

    }
}