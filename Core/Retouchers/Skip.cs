
using System.Linq;
using Chains;
using Core.Behaviors;

namespace Core.Retouchers
{
    public static class Skip
    {
        public static Retoucher EmptyAttack = new Retoucher(
            new ChainDef<Attacking.Event>("attack:check", new EvHandler<Attacking.Event>(SkipEmptyAttack))
        );
        public static Retoucher BlockedMove = new Retoucher(
            new ChainDef<Moving.Event>("move:check", new EvHandler<Moving.Event>(SkipBlocked))
        );
        public static Retoucher NoPlayer = new Retoucher(
            new ChainDef<Attacking.Event>("attack:check", new EvHandler<Attacking.Event>(SkipEmptyAttack))
        );
        public static Retoucher Self = new Retoucher(
            new ChainDef<Attacking.Event>("attack:check", new EvHandler<Attacking.Event>(SkipEmptyAttack))
        );

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