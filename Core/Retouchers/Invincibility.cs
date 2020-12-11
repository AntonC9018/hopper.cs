
using System.Linq;
using Hopper.Utils.Chains;
using Hopper.Core.Behaviors;

namespace Hopper.Core.Retouchers
{
    public class Invincibility
    {
        public Retoucher PreventsDamage = Retoucher
            .SingleHandlered(Attackable.Do, PreventDamage);
        public Retoucher Decreases = Retoucher
            .SingleHandlered(Tick.Chain, Decrease);

        public void RegisterAll(Registry registry)
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