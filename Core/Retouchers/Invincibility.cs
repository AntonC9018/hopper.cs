
using System.Linq;
using Chains;
using Core.Behaviors;

namespace Core.Retouchers
{
    public static class Invincibility
    {
        public static Retoucher PreventsDamage = Retoucher
            .SingleHandlered<Attackable.Event>("attacked:do", PreventDamage);
        public static Retoucher Decreases = Retoucher
            .SingleHandlered<CommonEvent>("tick", Decrease);


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