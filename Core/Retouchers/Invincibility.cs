
using System.Linq;
using Chains;
using Hopper.Core.Behaviors;

namespace Hopper.Core.Retouchers
{
    public static class Invincibility
    {
        public static Retoucher PreventsDamage = Retoucher
            .SingleHandlered<Attackable.Event>(Attackable.Do, PreventDamage);
        public static Retoucher Decreases = Retoucher
            .SingleHandlered<Tick.Event>(Tick.Chain, Decrease);


        // TODO:
        static void PreventDamage(Attackable.Event ev)
        {
        }
        // TODO:
        static void Decrease(Tick.Event ev)
        {
        }

    }
}