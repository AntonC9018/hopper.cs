using System.Collections.Generic;
using Chains;
using Core;
using Core.Behaviors;
using Core.Targeting;
using Utils.Vector;

namespace Test
{
    public class BindStatuses
    {
        public static BindStatus NoMove = CreateStopMoveBindStatus();
        // public static BindStatus NoMoveStatus = CreateStopMoveBindStatus();
        // public static BindStatus NoMoveStatus = CreateStopMoveBindStatus();

        private static BindStatus CreateStopMoveBindStatus()
        {
            var lambdas = new BindStatuses();
            var builder = lambdas.CreateBaseBuilder();
            AddNoMove(builder);
            lambdas.status = new BindStatus(builder.ToStatic(), Bind.Path);
            return lambdas.status;
        }

        private static void AddNoMove(ChainDefBuilder builder)
        {
            builder.AddDef<Moving.Event>(Moving.Check)
                   .AddHandler(StopMove);
        }

        public ChainDefBuilder CreateBaseBuilder()
        {
            return new ChainDefBuilder()
                .AddDef<Attacking.Event>(Attacking.Check)
                .AddHandler(AttackJustMe, PriorityRanks.High)
                .AddDef<Tick.Event>(Tick.Chain)
                .AddHandler(SelfRemove, PriorityRanks.High)
                .AddDef<Displaceable.Event>(Displaceable.Do)
                .AddHandler(DisplaceMe, PriorityRanks.Low)
                .End();
        }

        public BindStatus status;

        private void AttackJustMe(Attacking.Event ev)
        {
            var target = new Target
            {
                targetEntity = status.GetStore(ev).whoApplied,
                direction = new IntVector2(0, 0)
            };
            ev.targets = new List<Target> { target };
        }

        private void SelfRemove(Tick.Event ev)
        {
            var statusData = status.GetStore(ev.actor);
            if (statusData.whoApplied == null || statusData.whoApplied.IsDead)
            {
                // ev.actor.Untink(tinker);
                statusData.amount = 0;
            }
        }

        private void DisplaceMe(Displaceable.Event ev)
        {
            var statusData = status.GetStore(ev);
            if (statusData.whoApplied != null)
            {
                statusData.whoApplied.Pos = ev.actor.Pos;
                statusData.whoApplied.History.Add(statusData.whoApplied, History.UpdateCode.displaced_do);
            }
        }

        private static void StopMove(Moving.Event ev)
        {
            ev.propagate = false;
        }
    }
}