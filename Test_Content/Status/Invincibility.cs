using Hopper.Core.Chains;
using Hopper.Core;
using Hopper.Core.Behaviors.Basic;
using Hopper.Utils.Chains;
using Hopper.Core.Stats;

namespace Hopper.Test_Content
{
    public class Invincibility : StatusFile
    {
        public Invincibility()
        {
            power = 10;
            amount = 2;
        }

        public static readonly SimpleStatPath<Invincibility> Path =
            new SimpleStatPath<Invincibility>("status/inv", new Invincibility { power = 10, amount = 1 });

        public static readonly Status<StatusData> Status = new Status<StatusData>(
            new ChainDefBuilder()
                .AddDef(Attackable.Do)
                .AddHandler(PreventDamage, PriorityMapping.High + 0x8100)
                .End()
                .ToStatic(),
            Invincibility.Path,
            0
        );

        static void PreventDamage(Attackable.Event ev)
        {
            ev.atkParams.attack.damage = 0;
        }
    }
}