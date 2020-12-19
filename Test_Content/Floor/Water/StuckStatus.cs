using Hopper.Core.Chains;
using Hopper.Utils.Chains;
using Hopper.Core;
using Hopper.Core.Behaviors.Basic;

namespace Hopper.Test_Content.Floor
{
    public class StuckStatus : Status<StuckData>
    {

        public static StuckStatus Create(int defaultResValue)
        {
            var lambdas = new Lambdas();
            var chainDefs = lambdas.CreateBuilder().ToStatic();
            var status = new StuckStatus(chainDefs, defaultResValue);
            lambdas.status = status;
            return status;
        }

        public StuckStatus(IChainDef[] chainDefs, int defaultResValue)
            : base(chainDefs, Stuck.Path, defaultResValue)
        {
        }

        protected override void UpdateAmount(StuckData data)
        {
        }

        private class Lambdas
        {
            public StuckStatus status;

            public ChainDefBuilder CreateBuilder()
            {
                return new ChainDefBuilder()
                    .AddDef(Attacking.Do)
                    .AddHandler(PreventActionAndDecreaseAmount, PriorityRanks.High)
                    .AddDef(Digging.Do)
                    .AddHandler(PreventActionAndDecreaseAmount, PriorityRanks.High)
                    .AddDef(Displaceable.Do)
                    .AddHandler(PreventActionAndDecreaseAmount, PriorityRanks.High)
                    .End();
            }

            private void PreventActionAndDecreaseAmount(ActorEvent ev)
            {
                if (status.IsApplied(ev.actor))
                {
                    status.m_tinker.GetStore(ev).amount--;
                    ev.propagate = false;
                }
            }
        }
    }
}