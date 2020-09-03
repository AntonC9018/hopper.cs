
using System.Linq;
using Chains;
using Core.Behaviors;

namespace Core.Retouchers
{
    public static class Invincibility
    {
        public static Retoucher PreventsDamage = Retoucher
            .SingleHandlered<Attackable.Event>(Attackable.Do.TemplatePath, PreventDamage);
        public static Retoucher Decreases = Retoucher
            .SingleHandlered<Tick.Event>(Tick.chain.TemplatePath, Decrease);


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