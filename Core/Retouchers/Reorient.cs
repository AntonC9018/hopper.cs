
using System.Linq;
using Chains;

namespace Core.Retouchers
{
    public static class Reorient
    {
        public static Retoucher OnMove = new Retoucher(
            new ChainDefinition("move:do", new WeightedEventHandler(AnyReorient))
        );
        public static Retoucher OnDisplace = new Retoucher(
            new ChainDefinition("displaced:do", new WeightedEventHandler(AnyReorient))
        );
        public static Retoucher OnActionSuccess = new Retoucher(
            new ChainDefinition("displaced:do", new WeightedEventHandler(AnyReorient))
        );
        public static Retoucher OnAttack = new Retoucher(
            new ChainDefinition("displaced:do", new WeightedEventHandler(AnyReorient))
        );

        static void AnyReorient(EventBase eventBase)
        {
            var ev = (CommonEvent)eventBase;
            if (ev.action.direction != null)
            {
                ev.actor.Reorient(ev.action.direction);
            }
        }
    }
}