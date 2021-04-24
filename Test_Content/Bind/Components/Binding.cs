using Hopper.Core;
using Hopper.Core.Stat;
using Hopper.Core.Components;
using Hopper.Utils.Vector;

namespace Hopper.TestContent.Bind
{
    public class Bind : StatusFile
    {
        public Bind()
        {
            power = 1;
            amount = System.Int32.MaxValue;
        }

        public static readonly SimpleStatPath<Bind> Path =
            new SimpleStatPath<Bind>("status/bind");
        public static readonly BindStatus StopMoveStatus =
            BindStatuses.CreateStopMoveBindStatus();
        public static readonly Retoucher StopMoveRetoucher =
            BindRetouchers.CreateBindRetoucher(Bind.StopMoveStatus);
    }

    public class Binding : IStandartActivateable
    {
        public class Context : StandartContext
        {
            public BindStatus bindStatus;
            public Entity applyTo;
        }

        public BindStatus config_bindStatus;

        public void Init(BindStatus bindStatus)
        {
            config_bindStatus = bindStatus;
        }

        public bool Activate(IntVector2 direction) => Activate(direction, null);
        public bool Activate(IntVector2 direction, Entity applyTo)
        {
            var ev = new Event
            {
                actor = m_entity,
                direction = direction,
                applyTo = applyTo,
                statusStat = config_bindStatus.GetStat(m_entity),
                bindStatus = config_bindStatus
            };
            return CheckDoCycle<Event>(ev);
        }

        static void GetTarget(Context ev)
        {
            if (ev.applyTo == null)
            {
                ev.applyTo = ev.actor
                     .GetCellRelative(ev.direction)
                    ?.GetEntityFromLayer(ev.direction, Layer.REAL);

                if (ev.applyTo == null)
                {
                    ev.propagate = false;
                }
            }
        }

        static void CheckCanBind(Context ev)
        {
            ev.propagate = ev.bindStatus.IsApplied(ev.applyTo) == false;
        }

        static void BindTarget(Context ev)
        {
            ev.propagate = ev.bindStatus.TryApply(ev.applyTo, new BindData(ev.actor), ev.statusStat);
        }
    }
}