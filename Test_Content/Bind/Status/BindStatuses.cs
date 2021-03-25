using System.Collections.Generic;
using Hopper.Core.Chains;
using Hopper.Utils.Chains;
using Hopper.Core.Components.Basic;
using Hopper.Core.History;
using Hopper.Core.Targeting;
using Hopper.Utils.Vector;

namespace Hopper.Test_Content.Bind
{
    public class BindStatuses
    {
        public static BindStatus CreateStopMoveBindStatus()
        {
            var lambdas = new Lambdas();
            var builder = lambdas.CreateBaseBuilder();
            AddNoMove(builder);
            lambdas.status = new BindStatus(builder.ToStatic(), Bind.Path);
            return lambdas.status;
        }

        public static void AddNoMove(ChainDefBuilder builder)
        {
            builder.AddDef(Moving.Check)
                   .AddHandler(Utils.Handlers.StopMove);
        }

        private class Lambdas
        {
            public ChainDefBuilder CreateBaseBuilder()
            {
                return new ChainDefBuilder()

                    .AddDef(Attacking.Check)
                    .AddHandler(AttackJustMe, PriorityRank.High)

                    .AddDef(Tick.Chain)
                    .AddHandler(SelfRemove, PriorityRank.High)

                    .AddDef(Displaceable.Do)
                    .AddHandler(DisplaceMe, PriorityRank.Low)

                    // TODO: ignore any source of attack but the basic. 

                    .End();
            }

            public BindStatus status;

            private void AttackJustMe(Attacking.Event ev)
            {
                ev.targets = new List<Target> {
                    new Target(status.m_tinker.GetStore(ev).whoApplied, IntVector2.Zero)
                };
            }

            private void SelfRemove(Tick.Event ev)
            {
                var statusData = status.m_tinker.GetStore(ev.actor);
                if (statusData.whoApplied == null || statusData.whoApplied.IsDead)
                {
                    statusData.amount = 0;
                }
            }

            private void DisplaceMe(Displaceable.Context ev)
            {
                var statusData = status.m_tinker.GetStore(ev);
                if (statusData.whoApplied != null)
                {
                    statusData.whoApplied.ResetPosInGrid(ev.actor.Pos);
                    statusData.whoApplied.History.Add(statusData.whoApplied, UpdateCode.displaced_do);
                }
            }
        }
    }
}