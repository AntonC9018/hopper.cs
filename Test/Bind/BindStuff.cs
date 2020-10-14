using System.Collections.Generic;
using Chains;
using Core;
using Core.Behaviors;
using Core.Targeting;
using Utils.Vector;

namespace Test
{
    public static class BindStuff
    {
        public static Status<FlavorTinkerData<BindFlavor>> status;
        public static Tinker<FlavorTinkerData<BindFlavor>> tinker;

        public static void AttackJustMe(Attacking.Event ev)
        {
            var flavor = tinker.GetStore(ev).flavor;
            var target = new Target
            {
                Entity = flavor.whoApplied,
                direction = new IntVector2(0, 0)
            };
            ev.targets = new List<Target> { target };
        }

        public static void SelfRemove(Tick.Event ev)
        {
            var flavor = tinker.GetStore(ev.actor).flavor;
            if (flavor.whoApplied == null || flavor.whoApplied.IsDead)
            {
                // ev.actor.Untink(tinker);
                flavor.amount = 0;
            }
        }

        public static void DisplaceMe(Displaceable.Event ev)
        {
            var flavor = tinker.GetStore(ev).flavor;
            if (flavor.whoApplied != null)
            {
                flavor.whoApplied.Pos = ev.actor.Pos;
                flavor.whoApplied.History.Add(flavor.whoApplied, History.UpdateCode.displaced_do);
            }
        }

        public static void SetupTinker()
        {
            var builder = new ChainDefBuilder()
                .AddDef<Attacking.Event>(Attacking.Check)
                .AddHandler(AttackJustMe, PriorityRanks.High)
                .AddDef<Tick.Event>(Tick.Chain)
                .AddHandler(SelfRemove, PriorityRanks.High)
                .AddDef<Displaceable.Event>(Displaceable.Do)
                .AddHandler(DisplaceMe, PriorityRanks.Low)
                .End();

            tinker = new Tinker<FlavorTinkerData<BindFlavor>>(builder.ToStatic());
            status = new Status<FlavorTinkerData<BindFlavor>>(tinker);
        }

        static BindStuff()
        {
            SetupTinker();
        }

        public static void StopMove(Moving.Event ev)
        {
            ev.propagate = false;
        }

        public static Tinker<TinkerData> StopMoveSpice = Tinker<TinkerData>
            .SingleHandlered<Moving.Event>(Moving.Check, StopMove, PriorityRanks.High);
    }
}