using Hopper.Core.Chains;
using Hopper.Utils.Chains;
using System.Runtime.Serialization;

namespace Hopper.Core.Behaviors.Basic
{
    [DataContract]
    public class Acting : Behavior
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
        private System.Func<Entity, Action> config_CalculateAction;
        private System.Action<Event> config_DoActionFunc;

        private void Init(Config config)
        {
            config_CalculateAction = config.CalculateAction;
            config_DoActionFunc = config.DoAction;
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
                config_DoActionFunc(ev);
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
            if (NextAction == null && config_CalculateAction != null)
            {
                NextAction = config_CalculateAction(m_entity);
            }
        }

        public static readonly ChainPaths<Acting, Event> Check;
        public static readonly ChainPaths<Acting, Event> Fail;
        public static readonly ChainPaths<Acting, Event> Success;

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