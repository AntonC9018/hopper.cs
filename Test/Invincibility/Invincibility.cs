using Chains;
using Core;
using Core.Behaviors;
using Core.Stats;

namespace Test
{

    public class Invincibility : StatusFile
    {
        public Invincibility()
        {
            power = 10;
            amount = 2;
        }

        public static readonly IStatPath<Invincibility> Path = new StatPath<Invincibility>("status/inv");

        public static Status<StatusData> status = new Status<StatusData>(
            new ChainDefBuilder()
                .AddDef<Attackable.Event>(Attackable.Do)
                .AddHandler(PreventDamage, PriorityRanks.High)
                .End()
                .ToStatic(),
            Invincibility.Path, // TODO: actually add this stat
            0
        );

        static void PreventDamage(Attackable.Event ev)
        {
            ev.attack.damage = 0;
        }
    }
}