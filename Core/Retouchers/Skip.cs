
using System.Linq;
using Hopper.Utils.Chains;
using Hopper.Core.Behaviors.Basic;
using Hopper.Core.Targeting;

namespace Hopper.Core.Retouchers
{
    public class Skip
    {
        public Retoucher EmptyAttack = Retoucher
            .SingleHandlered<Attacking.Event>(Attacking.Check, SkipEmptyAttack);
        public Retoucher EmptyDig = Retoucher
            .SingleHandlered<Digging.Event>(Digging.Check, SkipEmptyDig);
        public Retoucher BlockedMove = Retoucher
            .SingleHandlered<Moving.Event>(Moving.Check, SkipBlocked);
        public Retoucher NoPlayer = Retoucher
            .SingleHandlered<Attacking.Event>(Attacking.Check, SkipNoPlayer);
        public Retoucher Self = Retoucher
            .SingleHandlered<Attacking.Event>(Attacking.Check, SkipSelf);

        public void RegisterAll(Registry registry)
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