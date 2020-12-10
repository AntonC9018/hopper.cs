
using System.Linq;
using Hopper.Utils.Chains;
using Hopper.Core.Behaviors;
using Hopper.Core.Targeting;

namespace Hopper.Core.Retouchers
{
    public static class Skip
    {
        public static Retoucher EmptyAttack = Retoucher
            .SingleHandlered<Attacking.Event>(Attacking.Check, SkipEmptyAttack);
        public static Retoucher EmptyDig = Retoucher
            .SingleHandlered<Digging.Event>(Digging.Check, SkipEmptyDig);
        public static Retoucher BlockedMove = Retoucher
            .SingleHandlered<Moving.Event>(Moving.Check, SkipBlocked);
        public static Retoucher NoPlayer = Retoucher
            .SingleHandlered<Attacking.Event>(Attacking.Check, SkipNoPlayer);
        public static Retoucher Self = Retoucher
            .SingleHandlered<Attacking.Event>(Attacking.Check, SkipSelf);

        static void SkipEmptyAttack(Attacking.Event ev)
        {
            ev.propagate = ev.targets.Count > 0;
        }

        static void SkipEmptyDig(Digging.Event ev)
        {
            ev.propagate = ev.targets.Count > 0;
        }

        static void SkipBlocked(Moving.Event ev)
        {
            ev.propagate = ev.actor.HasBlockRelative(ev.action.direction) == false;
        }

        static void SkipNoPlayer(Attacking.Event ev)
        {
            ev.propagate = ev.targets
                .Any(t => t.entity.IsPlayer);
        }

        static void SkipSelf(Attacking.Event ev)
        {
            ev.propagate = ev.targets
                .Any(t => t.entity == ev.actor);
        }
    }
}