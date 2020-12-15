using Hopper.Core.Registry;
using Hopper.Core.Behaviors.Basic;

namespace Hopper.Core.Retouchers
{
    public static class Invincibility
    {
        public static readonly Retoucher PreventsDamage = Retoucher.SingleHandlered(Attackable.Do, PreventDamage);
        public static readonly Retoucher Decreases = Retoucher.SingleHandlered(Tick.Chain, Decrease);

        public static void RegisterAll(ModSubRegistry registry)
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