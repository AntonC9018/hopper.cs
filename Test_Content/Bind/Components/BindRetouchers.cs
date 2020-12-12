using Hopper.Core.Chains;
using Hopper.Core;
using Hopper.Utils.Chains;
using Hopper.Core.Behaviors.Basic;

namespace Hopper.Test_Content.Bind
{
    public static class BindRetouchers
    {
        public static Retoucher CreateBindRetoucher(BindStatus bindStatus)
        {
            var lambdas = new Lambdas(bindStatus);
            var builder = new TemplateChainDefBuilder()

                .AddDef<Tick.Event>(Tick.Chain)
                .AddHandler(FreeIfHostIsDead, PriorityRanks.High)

                .AddDef<Binding.Event>(Binding.Do)
                .AddHandler(lambdas.Register)

                .AddDef<Displaceable.Event>(Displaceable.Check)
                .AddHandler(SkipDisplaceIfBinding)

                .End();

            return new Retoucher(builder.ToStatic());
        }

        private class Lambdas
        {
            public BindStatus status;

            public Lambdas(BindStatus status)
            {
                this.status = status;
            }

            public void Register(Binding.Event ev)
            {
                bool success = status.IsApplied(ev.applyTo);

                if (success)
                {
                    ((ISelfBinder)ev.actor).BoundEntity = ev.applyTo;
                    ev.actor.ResetPosInGrid(ev.applyTo.Pos);
                }
                else
                {
                    ev.actor.Die();
                }
            }
        }

        private static void FreeIfHostIsDead(Tick.Event ev)
        {
            var boundEntity = ((ISelfBinder)ev.actor).BoundEntity;
            if (boundEntity != null && boundEntity.IsDead)
            {
                boundEntity = null;
                ev.actor.ResetInGrid();
            }
        }

        private static void SkipDisplaceIfBinding(Displaceable.Event ev)
        {
            var boundEntity = ((ISelfBinder)ev.actor).BoundEntity;
            if (boundEntity != null)
            {
                ev.propagate = false;
            }
        }
    }
}