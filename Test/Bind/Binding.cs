using Chains;
using Core;
using Core.Behaviors;
using Core.FS;

namespace Test
{
    public class Binding : Behavior, IStandartActivateable
    {
        static void SetupStats()
        {
            Directory baseDir = StatManager.DefaultFS.BaseDir;

            StatFile bind = new StatusFile
            {
                power = 1,
                amount = System.Int32.MaxValue
            };

            baseDir.AddFile("test_bind", bind);
        }

        public class Event : CommonEvent
        {
            public StatusFile statusStat;
            public BindStatus bindStatus;
            public Entity applyTo;
        }

        public class Config : BehaviorConfig
        {
            public BindStatus bindStatus;
        }

        public BindStatus config_bindStatus;

        public override void Init(Entity entity, BehaviorConfig config)
        {
            config_bindStatus = ((Config)config).bindStatus;
            m_entity = entity;
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
                ev.applyTo = ev.actor
                    .GetCellRelative(ev.action.direction)
                    .GetEntityFromLayer(Layer.REAL);
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

        public static ChainPaths<Binding, Event> Check;
        public static ChainPaths<Binding, Event> Do;
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

            SetupStats();
        }
    }
}