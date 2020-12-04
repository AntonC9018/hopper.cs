
using System.Linq;
using Chains;
using Core.Behaviors;
using Core.Utils.Vector;

namespace Core.Retouchers
{
    public static class Reorient
    {
        public static Retoucher OnMove = Retoucher
            .SingleHandlered<Moving.Event>(Moving.Do, AnyReorient, PriorityRanks.High);
        public static Retoucher OnDisplace = Retoucher
            .SingleHandlered<Displaceable.Event>(Displaceable.Do, AnyReorient, PriorityRanks.High);
        public static Retoucher OnActionSuccess = Retoucher
            .SingleHandlered<Displaceable.Event>(Displaceable.Do, AnyReorient, PriorityRanks.High);
        public static Retoucher OnAttack = Retoucher
            .SingleHandlered<Displaceable.Event>(Displaceable.Do, AnyReorient, PriorityRanks.High);

        static void AnyReorient(Displaceable.Event ev)
        {
            if (ev.dir != IntVector2.Zero)
            {
                ev.actor.Reorient(ev.dir);
            }
        }
        static void AnyReorient(StandartEvent ev)
        {
            if (ev.action.direction != IntVector2.Zero)
            {
                ev.actor.Reorient(ev.action.direction);
            }
        }
    }
}