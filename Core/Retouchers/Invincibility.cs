
using System.Linq;
using Chains;

namespace Core.Retouchers
{
    public static class Invinsibility
    {
        public static Retoucher PreventsDamage = new Retoucher(
            new ChainDefinition("attacked:do", new WeightedEventHandler(PreventDamage))
        );
        // public static Retoucher PreventsDamage = new Retoucher(
        //     new ChainDefinition("attacked:do", new WeightedEventHandler(PreventDamage))
        // );

        // TODO:
        static void PreventDamage(EventBase eventBase)
        {
            var ev = (Attackable.Event)eventBase;
            // ev.propagate = ev.actor
        }

        // TODO: think about how we do it
        // right now it is impossible via retouchers.
        // We've got to somehow add an end of turn event to the entity
        // I guess we should add some hooks onto the entity factory
        static void AddDecrease(Entity entity)
        {

        }

        // TODO:
        static void Decrease(Entity entity)
        {
            // entity
        }

    }
}