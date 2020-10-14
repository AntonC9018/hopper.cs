using Chains;
using Core;
using Core.Behaviors;
using Core.FS;

namespace Test
{
    public class Binding : Behavior
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
            public Params pars;
            public StatusParam statusParam;
            public Entity applyTo;
        }

        public class Params : ActivationParams
        {
            public Entity applyTo;
            public ITinker spice;
        }


        public bool Activate(Action action, Params pars)
        {
            var ev = new Event
            {
                actor = m_entity,
                action = action,
                pars = pars
            };
            return CheckDoCycle<Event>(ev);
        }

        static void SetFlavor(Event ev)
        {
            ev.statusParam = new StatusParam
            (
                statusId: BindStuff.status.Id,
                flavor: new BindFlavor(ev.actor, ev.pars.spice),
                statusStat: (StatusFile)ev.actor.StatManager.GetFile("test_bind")
            );
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
            ev.propagate = ev.applyTo.Behaviors.Has<Statused>()
                && (ev.applyTo.Tinkers.IsTinked(BindStuff.tinker) == false);
        }

        static void BindTarget(Event ev)
        {
            var pars = new Statused.Params
            {
                statusParams = new StatusParam[] { ev.statusParam }
            };
            ev.propagate = ev.applyTo.Behaviors.Get<Statused>().Activate(ev.action, pars);
        }

        public static ChainPaths<Binding, Event> Check;
        public static ChainPaths<Binding, Event> Do;
        static Binding()
        {
            Check = new ChainPaths<Binding, Event>(ChainName.Check);
            Do = new ChainPaths<Binding, Event>(ChainName.Do);

            var builder = new ChainTemplateBuilder()

                .AddTemplate<Event>(ChainName.Check)
                .AddHandler(SetFlavor)
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