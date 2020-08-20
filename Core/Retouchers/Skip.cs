
using System.Linq;
using Chains;

namespace Core.Retouchers
{
    public static class Skip
    {
        public static Retoucher EmptyAttack = new Retoucher(
            new ChainDefinition("attack:check", new WeightedEventHandler(SkipEmptyAttack))
        );
        public static Retoucher BlockedMove = new Retoucher(
            new ChainDefinition("move:check", new WeightedEventHandler(SkipBlocked))
        );
        public static Retoucher NoPlayer = new Retoucher(
            new ChainDefinition("attack:check", new WeightedEventHandler(SkipEmptyAttack))
        );
        public static Retoucher Self = new Retoucher(
            new ChainDefinition("attack:check", new WeightedEventHandler(SkipEmptyAttack))
        );

        static void SkipEmptyAttack(EventBase e)
        {
            var ev = (Attacking.Event)e;
            ev.propagate = ev.targets.Count > 0;
        }

        static void SkipBlocked(EventBase e)
        {
            var ev = (Moving.Event)e;
            var block = ev.actor
                .GetCellRelative(ev.action.direction)
                .GetEntityFromLayer(Layer.BLOCK);
            ev.propagate = block == null;
        }

        static void SkipNoPlayer(EventBase e)
        {
            var ev = (Attacking.Event)e;
            ev.propagate = ev.targets
                .Any(t => t.entity.IsPlayer());
        }

        static void SkipSelf(EventBase e)
        {
            var ev = (Attacking.Event)e;
            ev.propagate = ev.targets
                .Any(t => t.entity == ev.actor);
        }
    }
}