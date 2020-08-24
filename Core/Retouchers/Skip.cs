
using System.Linq;
using Chains;

namespace Core.Retouchers
{
    public static class Skip
    {
        public static Retoucher EmptyAttack = new Retoucher(
            new ChainDef<CommonEvent>("attack:check", new EvHandler<CommonEvent>(SkipEmptyAttack))
        );
        public static Retoucher BlockedMove = new Retoucher(
            new ChainDef<CommonEvent>("move:check", new EvHandler<CommonEvent>(SkipBlocked))
        );
        public static Retoucher NoPlayer = new Retoucher(
            new ChainDef<CommonEvent>("attack:check", new EvHandler<CommonEvent>(SkipEmptyAttack))
        );
        public static Retoucher Self = new Retoucher(
            new ChainDef<CommonEvent>("attack:check", new EvHandler<CommonEvent>(SkipEmptyAttack))
        );

        static void SkipEmptyAttack(EventBase eventBase)
        {
            var ev = (Attacking.Event)eventBase;
            ev.propagate = ev.targets.Count > 0;
        }

        static void SkipBlocked(EventBase eventBase)
        {
            var ev = (Moving.Event)eventBase;
            var block = ev.actor
                .GetCellRelative(ev.action.direction)
                .GetEntityFromLayer(Layer.BLOCK);
            ev.propagate = block == null;
        }

        static void SkipNoPlayer(EventBase eventBase)
        {
            var ev = (Attacking.Event)eventBase;
            ev.propagate = ev.targets
                .Any(t => t.entity.IsPlayer());
        }

        static void SkipSelf(EventBase eventBase)
        {
            var ev = (Attacking.Event)eventBase;
            ev.propagate = ev.targets
                .Any(t => t.entity == ev.actor);
        }
    }
}