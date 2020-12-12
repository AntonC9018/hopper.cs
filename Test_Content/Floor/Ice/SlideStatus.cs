using Hopper.Core.Chains;
using Hopper.Core;
using Hopper.Core.Behaviors.Basic;
using Hopper.Utils.Chains;
using Hopper.Core.Stats.Basic;

namespace Hopper.Test_Content.Floor
{
    public class SlideStatus : Status<SlideData>
    {
        public static SlideStatus Create(int defaultResValue)
        {
            var lambdas = new Lambdas();
            var chainDefs = lambdas.CreateBuilder().ToStatic();
            var status = new SlideStatus(chainDefs, defaultResValue);
            lambdas.status = status;
            return status;
        }

        public SlideStatus(IChainDef[] chainDefs, int defaultResValue)
            : base(chainDefs, Floor.Slide.Path, defaultResValue)
        {
        }

        public override void Update(Entity entity)
        {
            if (m_tinker.IsTinked(entity))
            {
                var store = m_tinker.GetStore(entity);

                if (HasIceUnder(entity) == false)
                {
                    Remove(entity);
                }
            }
        }

        private bool HasIceUnder(Entity entity)
        {
            var floor = entity.GetCell().GetAnyEntityFromLayer(Layer.FLOOR);
            return floor != null && floor.Behaviors.Has<Sliding>();
        }

        // if we change direction during sliding, apply the newest one
        protected override void Reapply(SlideData existingData, SlideData newData)
        {
            existingData.initialDirection = newData.initialDirection;
        }

        private class Lambdas
        {
            public SlideStatus status;

            private void SlideInstead(ActorEvent ev)
            {
                // make all other entities that we're sliding into 
                // potentially slide out of the way first
                MakeEntitiesOnTheWaySlide(ev);

                // try to slide ourselves
                SlideIfDidnt(ev.actor);

                // remove the sliding status if there is anything in the way
                if (Sliding.IsWayFree(ev.actor) == false)
                {
                    status.Remove(ev.actor);
                }
            }

            private void MakeEntitiesOnTheWaySlide(ActorEvent ev)
            {
                var store = status.Tinker.GetStore(ev.actor);

                var entitiesOnTheWay = ev.actor
                    .GetCellRelative(store.initialDirection)
                    .GetAllFromLayer(Sliding.TargetedLayer);

                foreach (var thing in entitiesOnTheWay)
                {
                    if (status.IsApplied(thing))
                    {
                        SlideIfDidnt(thing);
                    }
                }
            }

            private void SlideIfDidnt(Entity actor)
            {
                var store = status.Tinker.GetStore(actor);
                if (store.didSlide == false)
                {
                    Slide(actor, store);
                }
            }

            private void Slide(Entity actor, SlideData store)
            {
                store.didSlide = true;

                var displaceable = actor.Behaviors.TryGet<Displaceable>();

                if (displaceable != null)
                {
                    var move = (Move)Move.Path.defaultFile.Copy();
                    displaceable.Activate(store.initialDirection, move);
                }
            }

            private void NoAction(ActorEvent ev)
            {
                ev.propagate = false;
            }

            private void ResetDidSlide(ActorEvent ev)
            {
                if (status.Tinker.IsTinked(ev.actor))
                {
                    status.Tinker.GetStore(ev.actor).didSlide = false;
                }
            }

            public ChainDefBuilder CreateBuilder() => new ChainDefBuilder()

                .AddDef(Attacking.Do)
                .AddHandler(NoAction, PriorityRanks.High)

                .AddDef(Digging.Do)
                .AddHandler(NoAction, PriorityRanks.High)

                .AddDef(Moving.Do)
                .AddHandler(NoAction, PriorityRanks.High)

                .AddDef(Acting.Success)
                .AddHandler(SlideInstead, PriorityRanks.High)

                .AddDef(Acting.Fail)
                .AddHandler(SlideInstead, PriorityRanks.High)

                .AddDef(Tick.Chain)
                .AddHandler(ResetDidSlide, PriorityRanks.High)

                .End();
        }
    }
}