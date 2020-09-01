
using System.Linq;
using Chains;
using Core.Behaviors;

namespace Core.Retouchers
{
    public static class Skip
    {
        public static Retoucher EmptyAttack = Retoucher
            .SingleHandlered<Attacking.Event>(Attacking.s_checkChainName, SkipEmptyAttack);
        public static Retoucher BlockedMove = Retoucher
            .SingleHandlered<Moving.Event>(Moving.s_checkChainName, SkipBlocked);
        public static Retoucher NoPlayer = Retoucher
            .SingleHandlered<Attacking.Event>(Attacking.s_checkChainName, SkipNoPlayer);
        public static Retoucher Self = Retoucher
            .SingleHandlered<Attacking.Event>(Attacking.s_checkChainName, SkipSelf);

        // public static void Setup()
        // {
        //     var emptyAttack = new ChainDefBuilder<Attacking.Event>(s_checkChainName);
        //     var blockedMove = new ChainDefBuilder<Moving.Event>(Moving.s_checkChainName);
        //     var noPlayer = new ChainDefBuilder<Attacking.Event>(s_checkChainName);
        //     var self = new ChainDefBuilder<Attacking.Event>(s_checkChainName);

        //     var attackHandler = new EvHandler<Attacking.Event>(SkipEmptyAttack);
        //     var blockedHandler = new EvHandler<Moving.Event>(SkipBlocked);
        //     var noPlayerHandler = new EvHandler<Attacking.Event>(SkipNoPlayer);
        //     var selfHandler = new EvHandler<Attacking.Event>(SkipSelf);

        //     emptyAttack.AddHandler(attackHandler);
        //     blockedMove.AddHandler(blockedHandler);
        //     noPlayer.AddHandler(noPlayerHandler);
        //     self.AddHandler(selfHandler);

        //     EmptyAttack = new Retoucher(emptyAttack.ToStatic());
        //     BlockedMove = new Retoucher(blockedMove.ToStatic());
        //     NoPlayer = new Retoucher(noPlayer.ToStatic());
        //     Self = new Retoucher(self.ToStatic());
        // }

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