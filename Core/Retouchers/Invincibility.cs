
using System.Linq;
using Chains;
using Core.Behaviors;

namespace Core.Retouchers
{
    public static class Invincibility
    {
        public static Retoucher PreventsDamage = Retoucher
            .SingleHandlered<Attackable.Event>(Attackable.s_doChainName, PreventDamage);
        public static Retoucher Decreases = Retoucher
            .SingleHandlered<Tick.Event>(Tick.m_chainName, Decrease);


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