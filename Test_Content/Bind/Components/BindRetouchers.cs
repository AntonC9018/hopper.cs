using Hopper.Core.Chains;
using Hopper.Core;
using Hopper.Utils.Chains;
using Hopper.Core.Behaviors.Basic;
using System;

namespace Hopper.Test_Content.Bind
{
    public static class BindRetouchers
    {
        public static Retoucher CreateBindRetoucher(BindStatus bindStatus)
        {
            var lambdas = new Lambdas(bindStatus);
            var builder = new TemplateChainDefBuilder()

                .AddDef(Tick.Chain)
                .AddHandler(FreeIfHostIsDead, PriorityRank.High)

                .AddDef(Binding.Do)
                .AddHandler(lambdas.Register)

                .AddDef(Displaceable.Check)
                .AddHandler(SkipDisplaceIfBinding)

                .End();

            return new Retoucher(builder.ToStatic());
        }

        private static void NoExplosion(Attackable.Event ev)
        {
            System.Console.WriteLine(ev.atkParams.attack.sourceId);
            if (ev.atkParams.attack.sourceId == Explosion.Explosion.AtkSource.Id)
            {
                ev.atkParams.attack.damage = 0;
            }
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
                if (status.IsApplied(ev.applyTo))
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