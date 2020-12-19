using Hopper.Core.Registries;
using System.Linq;
using Hopper.Core.Behaviors.Basic;
using Hopper.Utils.Chains;

namespace Hopper.Core.Retouchers
{
    public static class Skip
    {
        public static readonly Retoucher EmptyAttack = Retoucher
            .SingleHandlered<Attacking.Event>(Attacking.Check, SkipEmptyAttack, PriorityRank.Low);
        public static readonly Retoucher EmptyDig = Retoucher
            .SingleHandlered<Digging.Event>(Digging.Check, SkipEmptyDig, PriorityRank.Low);
        public static readonly Retoucher BlockedMove = Retoucher
            .SingleHandlered<Moving.Event>(Moving.Check, SkipBlocked, PriorityRank.Low);
        public static readonly Retoucher NoPlayer = Retoucher
            .SingleHandlered<Attacking.Event>(Attacking.Check, SkipNoPlayer, PriorityRank.Low);
        public static readonly Retoucher Self = Retoucher
            .SingleHandlered<Attacking.Event>(Attacking.Check, SkipSelf, PriorityRank.Low);

        public static void RegisterAll(ModRegistry registry)
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
            ev.propagate = ev.targets.Any(t => t.entity.IsPlayer);
        }

        private static void SkipSelf(Attacking.Event ev)
        {
            ev.propagate = ev.targets
                .Any(t => t.entity == ev.actor);
        }
    }
}