using Chains;
using Core;
using Core.Behaviors;

namespace Test
{

    public class InvincibilityData : FlavorTinkerData<Flavor>
    {
    }

    public static class InvincibilityStuff
    {
        public static Tinker<InvincibilityData> tinker = Tinker<InvincibilityData>
            .SingleHandlered<Attackable.Event>(Attackable.Do, PreventDamage, PriorityRanks.High);
        public static Status<InvincibilityData> status = new Status<InvincibilityData>(tinker);

        static void PreventDamage(Attackable.Event ev)
        {
            ev.attack.damage = 0;
        }
    }
}