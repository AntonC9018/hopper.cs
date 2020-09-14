
using System.Linq;
using Chains;
using Core.Behaviors;

namespace Core.Retouchers
{
    public static class Skip
    {
        public static Retoucher EmptyAttack = Retoucher
            .SingleHandlered<Attacking.Event>(Attacking.Check.TemplatePath, SkipEmptyAttack);
        public static Retoucher BlockedMove = Retoucher
            .SingleHandlered<Moving.Event>(Moving.Check.TemplatePath, SkipBlocked);
        public static Retoucher NoPlayer = Retoucher
            .SingleHandlered<Attacking.Event>(Attacking.Check.TemplatePath, SkipNoPlayer);
        public static Retoucher Self = Retoucher
            .SingleHandlered<Attacking.Event>(Attacking.Check.TemplatePath, SkipSelf);

        static void SkipEmptyAttack(Attacking.Event ev)
        {
            ev.propagate = ev.targets.Count > 0;
        }

        static void SkipBlocked(Moving.Event ev)
        {
            var block = ev.actor
                .GetCellRelative(ev.action.direction)
                .GetEntityFromLayer(Layer.BLOCK);
            ev.propagate = block == null;
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