
using System.Linq;
using Chains;

namespace Core.Retouchers
{
    public static class Reorient
    {
        public static Retoucher OnMove = new Retoucher(
            new ChainDef<CommonEvent>("move:do", new EvHandler<CommonEvent>(AnyReorient))
        );
        public static Retoucher OnDisplace = new Retoucher(
            new ChainDef<CommonEvent>("displaced:do", new EvHandler<CommonEvent>(AnyReorient))
        );
        public static Retoucher OnActionSuccess = new Retoucher(
            new ChainDef<CommonEvent>("displaced:do", new EvHandler<CommonEvent>(AnyReorient))
        );
        public static Retoucher OnAttack = new Retoucher(
            new ChainDef<CommonEvent>("displaced:do", new EvHandler<CommonEvent>(AnyReorient))
        );

        static void AnyReorient(CommonEvent ev)
        {
            if (ev.action.direction != null)
            {
                ev.actor.Reorient(ev.action.direction);
            }
        }
    }
}