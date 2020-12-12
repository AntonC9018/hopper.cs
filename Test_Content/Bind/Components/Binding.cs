using Hopper.Core.Chains;
using Hopper.Core;
using Hopper.Core.Stats;
using Hopper.Core.Behaviors;

namespace Hopper.Test_Content.Bind
{
    public class Bind : StatusFile
    {
        public Bind()
        {
            power = 1;
            amount = System.Int32.MaxValue;
        }

        public static readonly SimpleStatPath<Bind> Path = new SimpleStatPath<Bind>("status/bind");
    }

    public class Binding : Behavior, IStandartActivateable
    {
        public class Event : StandartEvent
        {
            public StatusFile statusStat;
            public BindStatus bindStatus;
            public Entity applyTo;
        }

        public class Config
        {
            public BindStatus bindStatus;
        }

        public BindStatus config_bindStatus;

        private void Init(Config config)
        {
            config_bindStatus = config.bindStatus;
        }

        public bool Activate(Action action) => Activate(action, null);
        public bool Activate(Action action, Entity applyTo)
        {
            var ev = new Event
            {
                actor = m_entity,
                action = action,
                applyTo = applyTo,
                statusStat = config_bindStatus.GetStat(m_entity),
                bindStatus = config_bindStatus
            };
            return CheckDoCycle<Event>(ev);
        }

        static void GetTarget(Event ev)
        {
            if (ev.applyTo == null)
            {
                ev.applyTo = ev.actor.GetCell().GetEntityFromLayer(ev.action.direction, Layer.REAL);
            }
        }

        static void CheckCanBind(Event ev)
        {
            ev.propagate = ev.bindStatus.IsApplied(ev.applyTo) == false;
        }

        static void BindTarget(Event ev)
        {
            ev.propagate = ev.bindStatus.TryApply(ev.applyTo, new BindData(ev.actor), ev.statusStat);
        }

        public static readonly ChainPaths<Binding, Event> Check;
        public static readonly ChainPaths<Binding, Event> Do;
        static Binding()
        {
            Check = new ChainPaths<Binding, Event>(ChainName.Check);
            Do = new ChainPaths<Binding, Event>(ChainName.Do);

            var builder = new ChainTemplateBuilder()

                .AddTemplate<Event>(ChainName.Check)
                .AddHandler(GetTarget)
                .AddHandler(CheckCanBind)

                .AddTemplate<Event>(ChainName.Do)
                .AddHandler(BindTarget)
                //  .AddHandler(Utils.AddHistoryEvent(History.EventCode.pushed_do))
                .End();

            BehaviorFactory<Binding>.s_builder = builder;
        }
    }
}