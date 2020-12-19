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
                if (DoesNotHaveIceUnder(entity))
                {
                    Remove(entity);
                }
            }
        }

        private bool DoesNotHaveIceUnder(Entity entity)
        {
            foreach (var potentiallyIce in entity.GetCell().m_entities)
            {
                if (potentiallyIce.Behaviors.Has<Sliding>())
                {
                    return false;
                }
            }
            return true;
        }

        // if we change direction during sliding, apply the newest one
        protected override void Reapply(SlideData existingData, SlideData newData)
        {
            existingData.currentDirection = newData.currentDirection;
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
                SlideIfHasNot(ev.actor);

                // remove the sliding status if there is anything in the way
                if (Sliding.IsWayFree(ev.actor) == false)
                {
                    status.Remove(ev.actor);
                }
            }

            private void MakeEntitiesOnTheWaySlide(ActorEvent ev)
            {
                var store = status.m_tinker.GetStore(ev.actor);

                var entitiesOnTheWay = ev.actor
                    .GetCellRelative(store.currentDirection)
                    .GetAllFromLayer(Sliding.TargetedLayer);

                foreach (var thing in entitiesOnTheWay)
                {
                    if (status.IsApplied(thing))
                    {
                        SlideIfHasNot(thing);
                    }
                }
            }

            private void SlideIfHasNot(Entity actor)
            {
                var store = status.m_tinker.GetStore(actor);
                if (!store.didSlide)
                {
                    Slide(actor, store);
                }
            }

            private void Slide(Entity actor, SlideData store)
            {
                store.didSlide = true;

                if (actor.Behaviors.Has<Displaceable>())
                {
                    var move = (Move)Move.Path.defaultFile.Copy();
                    actor.Behaviors.Get<Displaceable>().Activate(store.currentDirection, move);
                }
            }

            private void NoAction(Controllable.Event ev)
            {
                ev.action = null;
                ev.propagate = false;
            }

            private void ResetDidSlide(ActorEvent ev)
            {
                if (status.m_tinker.IsTinked(ev.actor))
                {
                    status.m_tinker.GetStore(ev.actor).didSlide = false;
                }
            }

            // private void ChangeDirection(Pushable.Event ev)
            // {
            //     if (status.m_tinker.IsTinked(ev.actor))
            //     {
            //         status.m_tinker.GetStore(ev.actor).currentDirection = ev.dir;
            //     }
            // }


            public ChainDefBuilder CreateBuilder() => new ChainDefBuilder()

                .AddDef(Controllable.Chains[InputMapping.Vector])
                .AddHandler(NoAction, PriorityRank.High)

                .AddDef(Acting.Success)
                .AddHandler(SlideInstead, PriorityRank.High)

                .AddDef(Acting.Fail)
                .AddHandler(SlideInstead, PriorityRank.High)

                .AddDef(Tick.Chain)
                .AddHandler(ResetDidSlide, PriorityRank.High)

                // .AddDef(Pushable.Do)
                // .AddHandler(ChangeDirection, PriorityRanks.Low)

                .End();
        }
    }
}