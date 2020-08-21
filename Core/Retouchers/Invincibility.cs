
using System.Linq;
using Chains;

namespace Core.Retouchers
{
    public static class Invinsibility
    {
        public static Retoucher PreventsDamage = new Retoucher(
            new ChainDefinition("attacked:do", new WeightedEventHandler(PreventDamage))
        );
        public static Retoucher Decreases = new Retoucher(
            new ChainDefinition("tick", new WeightedEventHandler(Decrease))
        );

        // TODO:
        static void PreventDamage(EventBase eventBase)
        {
            var ev = (Attackable.Event)eventBase;
            // ev.propagate = ev.actor
        }
        // TODO:
        static void Decrease(EventBase tickEvent)
        {
            var ev = (Entity.TickEvent)tickEvent;
            // entity
        }

    }
}