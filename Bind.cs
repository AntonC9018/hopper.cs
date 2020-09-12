using System.Collections.Generic;
using Chains;
using Core;
using Core.Behaviors;
using Core.FS;
using Vector;

namespace Test
{
    // TODO: really think through how to (de)serialize this
    public interface ISelfBinder
    {
        public Entity BoundEntity { get; set; }
    }

    public class Spider : Entity, ISelfBinder
    {
        public Spider() : base() { }

        public static EntityFactory<Spider> Factory;

        public Entity BoundEntity { get; set; }

        static StepData[] CreateSequenceData()
        {
            var bindAction = new SimpleAction(
                (entity, a) => entity.GetBehavior<Binding>()
                    .Activate(a, new Binding.Params { spice = BindStuff.StopMoveSpice })
            );

            var moveAction = new BehaviorAction<Moving>();

            var bindMoveAction = new CompositeAction(
                new Action[] { bindAction, moveAction }
            );

            var stepData = new StepData[]
            {
                new StepData
                {
                    action = bindMoveAction,
                    movs = Movs.Diagonal,
                    relativeStepIndexFail = 0,
                    successFunction = e => new Result
                    {
                        success = ((ISelfBinder)e).BoundEntity != null
                    }
                },
                new StepData
                {
                    action = null,
                    successFunction = e => new Result
                    {
                        success = ((ISelfBinder)e).BoundEntity != null,
                        index = 0
                    },
                    relativeStepIndexFail = 1
                }
            };

            return stepData;
        }

        static Spider()
        {
            Factory = new EntityFactory<Spider>();
            Factory.AddBehavior<Acting>(new Acting.Config { DoAction = Algos.EnemyAlgo });
            Factory.AddBehavior<Sequential>(new Sequential.Config(CreateSequenceData()));
            Factory.AddBehavior<Displaceable>();
            Factory.AddBehavior<Binding>();
            Factory.AddBehavior<Attackable>();
            Factory.RetouchAndSave(SelfBindingStuff.retoucher);
        }
    }

    public class BindFlavor : Flavor, IHaveSpice
    {
        public Entity whoApplied;

        public ITinker spice;
        public ITinker Spice => spice;

        public BindFlavor(Entity whoApplied, ITinker spice)
        {
            this.whoApplied = whoApplied;
            this.spice = spice;
        }

    }

    public static class BindStuff
    {
        public static Status<FlavorTinkerData<BindFlavor>> status;
        public static Tinker<FlavorTinkerData<BindFlavor>> tinker;

        // public static System.Action<E> FlavorWrap<E, F>(System.Action<E, F> func)
        //     where E : CommonEvent
        //     where F : Flavor
        // {
        //     return e => func(e, (F)tinker.GetStoreByEvent(e).flavor);
        // }

        public static void AttackJustMe(Attacking.Event ev)
        {
            var flavor = tinker.GetStore(ev).flavor;
            var target = new Target
            {
                entity = flavor.whoApplied,
                direction = new IntVector2(0, 0)
            };
            ev.targets = new List<Target> { target };
        }

        public static void SelfRemove(Tick.Event ev)
        {
            var flavor = tinker.GetStore(ev.actor.id).flavor;
            if (flavor.whoApplied == null || flavor.whoApplied.IsDead)
            {
                // ev.actor.Untink(tinker);
                flavor.amount = 0;
            }
        }

        public static void DisplaceMe(Displaceable.Event ev)
        {
            var flavor = tinker.GetStore(ev).flavor;
            if (flavor.whoApplied != null)
            {
                flavor.whoApplied.Pos = ev.actor.Pos;
                flavor.whoApplied.History.Add(flavor.whoApplied, History.EventCode.displaced_do);
            }
        }

        public static void SetupTinker()
        {
            var builder = new ChainDefsBuilder()
                .AddDef<Attacking.Event>(Attacking.Check.ChainPath)
                .AddHandler(AttackJustMe, PriorityRanks.High)
                .EndDef()
                .AddDef<Tick.Event>(Tick.chain.ChainPath)
                .AddHandler(SelfRemove, PriorityRanks.High)
                .EndDef()
                .AddDef<Displaceable.Event>(Displaceable.Do.ChainPath)
                .AddHandler(DisplaceMe, PriorityRanks.Low)
                .EndDef();

            tinker = new Tinker<FlavorTinkerData<BindFlavor>>(builder.ToStatic());
            status = new Status<FlavorTinkerData<BindFlavor>>(tinker);
        }

        static BindStuff()
        {
            SetupTinker();
        }

        public static void StopMove(Moving.Event ev)
        {
            ev.propagate = false;
        }

        public static Tinker<TinkerData> StopMoveSpice = Tinker<TinkerData>
            .SingleHandlered<Moving.Event>(Moving.Check.ChainPath, StopMove, PriorityRanks.High);
    }

    public static class SelfBindingStuff
    {
        public static Retoucher retoucher = SetupRetoucher();

        static void Register(Binding.Event ev)
        {
            bool success = ev.applyTo.IsTinked(BindStuff.tinker);

            if (success)
            {
                ((ISelfBinder)ev.actor).BoundEntity = ev.applyTo;
                ev.actor.ResetPosInGrid(ev.applyTo.Pos);
            }
            else
            {
                ev.actor.Die();
            }
        }

        static void FreeIfHostIsDead(Tick.Event ev)
        {
            var boundEntity = ((ISelfBinder)ev.actor).BoundEntity;
            if (boundEntity != null && boundEntity.IsDead)
            {
                boundEntity = null;
                ev.actor.ResetInGrid();
            }
        }

        static void SkipDisplaceIfBinding(Displaceable.Event ev)
        {
            var boundEntity = ((ISelfBinder)ev.actor).BoundEntity;
            if (boundEntity != null)
            {
                ev.propagate = false;
            }
        }

        public static Retoucher SetupRetoucher()
        {
            var builder = new TemplateChainDefsBuilder()
                .AddDef<Tick.Event>(Tick.chain.TemplatePath)
                .AddHandler(FreeIfHostIsDead, PriorityRanks.High)
                .EndDef()
                .AddDef<Binding.Event>(Binding.Do.TemplatePath)
                .AddHandler(Register)
                .EndDef()
                .AddDef<Displaceable.Event>(Displaceable.Check.TemplatePath)
                .AddHandler(SkipDisplaceIfBinding)
                .EndDef();

            return new Retoucher(builder.ToStatic());
        }
    }

    public class Binding : Behavior
    {
        public static int s_bindStatusIndex;
        static void SetupStats()
        {
            Directory baseDir = StatManager.DefaultFS.BaseDir;
            s_bindStatusIndex = Statused.RegisterStatus("Bind", BindStuff.status);

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
            public StatusData statusData;
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
            ev.statusData = new StatusData
            (
                statusIndex: s_bindStatusIndex,
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
            ev.propagate = ev.applyTo.HasBehavior<Statused>()
                && (ev.applyTo.IsTinked(BindStuff.tinker) == false);
        }

        static void BindTarget(Event ev)
        {
            var pars = new Statused.Params
            {
                statusData = new StatusData[] { ev.statusData }
            };
            ev.propagate = ev.applyTo.GetBehavior<Statused>().Activate(ev.action, pars);
        }

        public static ChainPaths<Binding, Event> Check;
        public static ChainPaths<Binding, Event> Do;
        static Binding()
        {
            var builder = new ChainTemplateBuilder();

            Check = new ChainPaths<Binding, Event>(ChainName.Check);
            builder
                .AddTemplate<Event>(ChainName.Check)
                .AddHandler(SetFlavor)
                .AddHandler(GetTarget)
                .AddHandler(CheckCanBind);


            Do = new ChainPaths<Binding, Event>(ChainName.Do);
            builder
                .AddTemplate<Event>(ChainName.Do)
                .AddHandler(BindTarget)
            //  .AddHandler(Utils.AddHistoryEvent(History.EventCode.pushed_do))
            ;

            BehaviorFactory<Binding>.s_builder = builder;

            SetupStats();
        }
    }
}