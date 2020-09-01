
using System.Linq;
using Chains;
using Core.Behaviors;

namespace Core.Retouchers
{
    public static class Reorient
    {
        public static Retoucher OnMove = Retoucher
            .SingleHandlered<CommonEvent>("move:do", AnyReorient);
        public static Retoucher OnDisplace = Retoucher
            .SingleHandlered<CommonEvent>("displaced:do", AnyReorient);
        public static Retoucher OnActionSuccess = Retoucher
            .SingleHandlered<CommonEvent>("displaced:do", AnyReorient);
        public static Retoucher OnAttack = Retoucher
            .SingleHandlered<CommonEvent>("displaced:do", AnyReorient);

        static void AnyReorient(CommonEvent ev)
        {
            if (ev.action.direction != null)
            {
                ev.actor.Reorient(ev.action.direction);
            }
        }
    }
}