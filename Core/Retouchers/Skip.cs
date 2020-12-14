using System.Linq;
using Hopper.Core.Behaviors.Basic;

namespace Hopper.Core.Retouchers
{
    public static class Skip
    {
        public static readonly Retoucher EmptyAttack = Retoucher
            .SingleHandlered<Attacking.Event>(Attacking.Check, SkipEmptyAttack);
        public static readonly Retoucher EmptyDig = Retoucher
            .SingleHandlered<Digging.Event>(Digging.Check, SkipEmptyDig);
        public static readonly Retoucher BlockedMove = Retoucher
            .SingleHandlered<Moving.Event>(Moving.Check, SkipBlocked);
        public static readonly Retoucher NoPlayer = Retoucher
            .SingleHandlered<Attacking.Event>(Attacking.Check, SkipNoPlayer);
        public static readonly Retoucher Self = Retoucher
            .SingleHandlered<Attacking.Event>(Attacking.Check, SkipSelf);

        public static void RegisterAll(ModSubRegistry registry)
        {
            EmptyAttack.RegisterSelf(registry);
            EmptyDig.RegisterSelf(registry);
            BlockedMove.RegisterSelf(registry);
            NoPlayer.RegisterSelf(registry);
            Self.RegisterSelf(registry);
        }

        private static void SkipEmptyAttack(Attacking.Event ev)
        {
            ev.propagate = ev.targets.Count > 0;
        }

        private static void SkipEmptyDig(Digging.Event ev)
        {
            ev.propagate = ev.targets.Count > 0;
        }

        private static void SkipBlocked(Moving.Event ev)
        {
            ev.propagate = ev.actor.HasBlockRelative(ev.action.direction) == false;
        }

        private static void SkipNoPlayer(Attacking.Event ev)
        {
            ev.propagate = ev.targets
                .Any(t => t.entity.IsPlayer);
        }

        private static void SkipSelf(Attacking.Event ev)
        {
            ev.propagate = ev.targets
                .Any(t => t.entity == ev.actor);
        }
    }
}