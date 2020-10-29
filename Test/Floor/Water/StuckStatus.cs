using Chains;
using Core;
using Core.Behaviors;

namespace Test
{
    public class StuckStatus : Status<StuckData>
    {
        public StuckStatus(int defaultResValue)
            : base(builder.ToStatic(), StuckStat.Path, defaultResValue)
        {
        }

        protected override void DecrementAmount(StuckData data)
        {
        }

        private static void PreventActionAndDecreaseAmount(ActorEvent ev)
        {
            if (Status.IsApplied(ev.actor))
            {
                Status.Tinker.GetStore(ev).amount--;
                ev.propagate = false;
            }
        }

        private static ChainDefBuilder builder = new ChainDefBuilder()
            .AddDef(Attacking.Do)
            .AddHandler(PreventActionAndDecreaseAmount, PriorityRanks.High)
            .AddDef(Digging.Do)
            .AddHandler(PreventActionAndDecreaseAmount, PriorityRanks.High)
            .AddDef(Displaceable.Do)
            .AddHandler(PreventActionAndDecreaseAmount, PriorityRanks.High)
            .End();

        public static readonly StuckStatus Status = new StuckStatus(1);
    }
}