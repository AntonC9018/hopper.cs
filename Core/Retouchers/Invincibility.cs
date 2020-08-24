
using System.Linq;
using Chains;

namespace Core.Retouchers
{
    public static class Invinsibility
    {
        public static Retoucher PreventsDamage = new Retoucher(
            new ChainDef<CommonEvent>("attacked:do", new EvHandler<CommonEvent>(PreventDamage))
        );
        public static Retoucher Decreases = new Retoucher(
            new ChainDef<CommonEvent>("tick", new EvHandler<CommonEvent>(Decrease))
        );

        // TODO:
        static void PreventDamage(CommonEvent commonEvent)
        {
            var ev = (Attackable.Event)commonEvent;
            // ev.propagate = ev.actor
        }
        // TODO:
        static void Decrease(CommonEvent commonEvent)
        {
            var ev = commonEvent;
            // entity
        }

    }
}