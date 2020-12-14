
using System.Linq;
using Hopper.Utils.Chains;
using Hopper.Core.Behaviors.Basic;
using Hopper.Utils.Vector;

namespace Hopper.Core.Retouchers
{
    public static class Reorient
    {
        public static readonly Retoucher OnMove = Retoucher
            .SingleHandlered(Moving.Do, AnyReorient, PriorityRanks.High);
        public static readonly Retoucher OnDisplace = Retoucher
            .SingleHandlered(Displaceable.Do, AnyReorient, PriorityRanks.High);
        public static readonly Retoucher OnActionSuccess = Retoucher
            .SingleHandlered(Displaceable.Do, AnyReorient, PriorityRanks.High);
        public static readonly Retoucher OnAttack = Retoucher
            .SingleHandlered(Displaceable.Do, AnyReorient, PriorityRanks.High);

        public static void RegisterAll(ModSubRegistry registry)
        {
            OnMove.RegisterSelf(registry);
            OnDisplace.RegisterSelf(registry);
            OnActionSuccess.RegisterSelf(registry);
            OnAttack.RegisterSelf(registry);
        }

        private static void AnyReorient(Displaceable.Event ev)
        {
            if (ev.dir != IntVector2.Zero)
            {
                ev.actor.Reorient_(ev.dir);
            }
        }
        private static void AnyReorient(StandartEvent ev)
        {
            if (ev.action.direction != IntVector2.Zero)
            {
                ev.actor.Reorient_(ev.action.direction);
            }
        }
    }
}