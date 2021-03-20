using Hopper.Core.Registries;
using Hopper.Utils.Chains;
using Hopper.Core.Behaviors.Basic;
using Hopper.Utils.Vector;

namespace Hopper.Core.Retouchers
{
    public static class Reorient
    {
        public static readonly Retoucher OnMove = Retoucher
            .SingleHandlered(Moving.Do, AnyReorient, PriorityRank.High);
        public static readonly Retoucher OnDisplace = Retoucher
            .SingleHandlered(Displaceable.Do, AnyReorient, PriorityRank.High);
        public static readonly Retoucher OnActionSuccess = Retoucher
            .SingleHandlered(Acting.Success, ActingReorient, PriorityRank.High);
        public static readonly Retoucher OnAttack = Retoucher
            .SingleHandlered(Displaceable.Do, AnyReorient, PriorityRank.High);
        public static readonly Retoucher OnActionSuccessToClosestPlayer = Retoucher
            .SingleHandlered(Acting.Success, ToPlayer, PriorityRank.High);

        public static void RegisterAll(ModRegistry registry)
        {
            OnMove.RegisterSelf(registry);
            OnDisplace.RegisterSelf(registry);
            OnActionSuccess.RegisterSelf(registry);
            OnAttack.RegisterSelf(registry);
            OnActionSuccessToClosestPlayer.RegisterSelf(registry);
        }

        private static void AnyReorient(StandartEvent ev)
        {
            if (ev.direction != IntVector2.Zero)
            {
                ev.actor.Orientation = ev.direction;
            }
        }

        private static void ActingReorient(Acting.Event ev)
        {
            if (ev.action is ParticularDirectedAction)
            {
                ev.actor.Orientation = ((ParticularDirectedAction)ev.action).direction;
            }
        }

        private static void ToPlayer(ActorEvent ev)
        {
            ToPlayer(ev.actor);
        }

        private static void ToPlayer(this Entity actor)
        {
            if (actor.TryGetClosestPlayer(out var player))
            {
                var diff = player.Pos - actor.Pos;
                var sign = diff.Sign();
                var abs = diff.Abs();
                if (abs.x > abs.y)
                {
                    actor.Orientation = new IntVector2(sign.x, 0);
                }
                if (abs.y > abs.x)
                {
                    actor.Orientation = new IntVector2(0, sign.y);
                }

            }
        }
    }
}