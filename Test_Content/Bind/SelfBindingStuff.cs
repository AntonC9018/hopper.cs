using Hopper.Core.Chains;
using Hopper.Core;
using Hopper.Utils.Chains;
using Hopper.Core.Behaviors;

namespace Hopper.Test_Content
{
    public class SelfBinding
    {
        public static Retoucher NoMoveRetoucher = SetupRetoucher(BindStatuses.NoMove);

        private BindStatus status;

        public SelfBinding(BindStatus status)
        {
            this.status = status;
        }

        public static Retoucher SetupRetoucher(BindStatus bindStatus)
        {
            var lambdas = new SelfBinding(bindStatus);
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


        private void Register(Binding.Event ev)
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