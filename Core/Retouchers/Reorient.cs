
using System.Linq;
using Chains;
using Core.Behaviors;

namespace Core.Retouchers
{
    public static class Reorient
    {
        public static Retoucher OnMove = Retoucher
            .SingleHandlered<Moving.Event>(Moving.Do.TemplatePath, AnyReorient);
        public static Retoucher OnDisplace = Retoucher
            .SingleHandlered<Displaceable.Event>(Displaceable.Do.TemplatePath, AnyReorient);
        public static Retoucher OnActionSuccess = Retoucher
            .SingleHandlered<Displaceable.Event>(Displaceable.Do.TemplatePath, AnyReorient);
        public static Retoucher OnAttack = Retoucher
            .SingleHandlered<Displaceable.Event>(Displaceable.Do.TemplatePath, AnyReorient);

        static void AnyReorient(CommonEvent ev)
        {
            if (ev.action.direction != null)
            {
                ev.actor.Reorient(ev.action.direction);
            }
        }
    }
}