using Chains;
using Core;
using Core.Behaviors;

namespace Test
{
    public static class SelfBindingStuff
    {
        public static Retoucher retoucher = SetupRetoucher();

        static void Register(Binding.Event ev)
        {
            bool success = ev.applyTo.Tinkers.IsTinked(BindStuff.tinker);

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

        static void FreeIfHostIsDead(Tick.Event ev)
        {
            var boundEntity = ((ISelfBinder)ev.actor).BoundEntity;
            if (boundEntity != null && boundEntity.IsDead)
            {
                boundEntity = null;
                ev.actor.ResetInGrid();
            }
        }

        static void SkipDisplaceIfBinding(Displaceable.Event ev)
        {
            var boundEntity = ((ISelfBinder)ev.actor).BoundEntity;
            if (boundEntity != null)
            {
                ev.propagate = false;
            }
        }

        public static Retoucher SetupRetoucher()
        {
            var builder = new TemplateChainDefBuilder()

                .AddDef<Tick.Event>(Tick.Chain)
                .AddHandler(FreeIfHostIsDead, PriorityRanks.High)

                .AddDef<Binding.Event>(Binding.Do)
                .AddHandler(Register)

                .AddDef<Displaceable.Event>(Displaceable.Check)
                .AddHandler(SkipDisplaceIfBinding)

                .End();

            return new Retoucher(builder.ToStatic());
        }
    }
}