using Chains;
using Core;
using Core.Behaviors;

namespace Test
{

    public static class Invincibility
    {
        // TODO: this probably should be a `positive status` = benefit
        // which get applied unconditionally
        public static Status<StatusData> status = new Status<StatusData>(
            new ChainDefBuilder()
                .AddDef<Attackable.Event>(Attackable.Do)
                .AddHandler(PreventDamage, PriorityRanks.High)
                .End()
                .ToStatic(),
            "test_invincibility", // TODO: actually add this stat
            0
        );

        static void PreventDamage(Attackable.Event ev)
        {
            ev.attack.damage = 0;
        }
    }
}