using Chains;
using Core;
using Core.Behaviors;
using Core.Stats.Basic;

namespace Test
{
    public class SlideStatus : Status<SlideData>
    {
        public SlideStatus(int defaultResValue)
            : base(builder.ToStatic(), SlideStat.Path, defaultResValue)
        {
        }

        public override void Update(Entity entity)
        {
            if (m_tinker.IsTinked(entity))
            {
                var store = m_tinker.GetStore(entity);

                if (HasIceUnder(entity) == false)
                {
                    m_tinker.Untink(entity);
                }
            }
        }

        private bool HasIceUnder(Entity entity)
        {
            var floor = entity.Cell.GetEntityFromLayer(Layer.FLOOR);
            return floor != null && floor.Behaviors.Has<Sliding>();
        }

        // if we change direction during sliding, apply the newest one
        protected override void Reapply(SlideData existingData, SlideData newData)
        {
            existingData.initialDirection = newData.initialDirection;
        }

        private static void SlideInstead(ActorEvent ev)
        {
            // make all other entities that we're sliding into 
            // potentially slide out of the way first
            MakeEntitiesOnTheWaySlide(ev);

            // try to slide ourselves
            SlideIfDidnt(ev.actor);

            // remove the sliding status if there is anything in the way
            if (Sliding.IsWayFree(ev.actor) == false)
            {
                Status.Tinker.Untink(ev.actor);
            }
        }

        private static void MakeEntitiesOnTheWaySlide(ActorEvent ev)
        {
            var store = Status.Tinker.GetStore(ev.actor);

            var entitiesOnTheWay = ev.actor
                .GetCellRelative(store.initialDirection)
                .GetAllFromLayer(Sliding.TargetedLayer);

            foreach (var thing in entitiesOnTheWay)
            {
                if (Status.IsApplied(thing))
                {
                    SlideIfDidnt(thing);
                }
            }
        }

        private static void SlideIfDidnt(Entity actor)
        {
            var store = Status.Tinker.GetStore(actor);
            if (store.didSlide == false)
            {
                Slide(actor, store);
            }
        }

        private static void Slide(Entity actor, SlideData store)
        {
            store.didSlide = true;

            var displaceable = actor.Behaviors.TryGet<Displaceable>();

            if (displaceable != null)
            {
                var move = (Move)Move.Path.DefaultFile.Copy();
                displaceable.Activate(store.initialDirection, move);
            }
        }

        private static void NoAction(ActorEvent ev)
        {
            ev.propagate = false;
        }

        private static void ResetDidSlide(ActorEvent ev)
        {
            if (Status.Tinker.IsTinked(ev.actor))
            {
                Status.Tinker.GetStore(ev.actor).didSlide = false;
            }
        }

        private static ChainDefBuilder builder = new ChainDefBuilder()

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

        public static SlideStatus Status = new SlideStatus(1);
    }
}