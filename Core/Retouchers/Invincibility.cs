
using System.Linq;
using Chains;
using Core.Behaviors;

namespace Core.Retouchers
{
    public static class Invinsibility
    {
        public static Retoucher PreventsDamage = new Retoucher(
            new ChainDef<Attackable.Event>("attacked:do", new EvHandler<Attackable.Event>(PreventDamage))
        );
        public static Retoucher Decreases = new Retoucher(
            new ChainDef<CommonEvent>("tick", new EvHandler<CommonEvent>(Decrease))
        );

        // TODO:
        static void PreventDamage(Attackable.Event ev)
        {
        }
        // TODO:
        static void Decrease(CommonEvent ev)
        {
        }

    }
}