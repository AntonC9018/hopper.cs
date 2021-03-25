using Hopper.Core.Registries;
using Hopper.Core.Components.Basic;

namespace Hopper.Core.Retouchers
{
    public static class Invincibility
    {
        public static readonly Retoucher PreventsDamage = Retoucher.SingleHandlered(Attackable.Do, PreventDamage);
        public static readonly Retoucher Decreases = Retoucher.SingleHandlered(Tick.Chain, Decrease);

        public static void RegisterAll(ModRegistry registry)
        {
            PreventsDamage.RegisterSelf(registry);
            Decreases.RegisterSelf(registry);
        }

        // TODO:
        private static void PreventDamage(Attackable.Event ev)
        {
        }
        // TODO:
        private static void Decrease(Tick.Event ev)
        {
        }

    }
}