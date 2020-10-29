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
            var store = Status.Tinker.GetStore(ev.actor);
            var prevPos = ev.actor.Pos;

            if (Sliding.IsWayFree(ev.actor))
            {
                var displaceable = ev.actor.Behaviors.TryGet<Displaceable>();

                if (displaceable != null)
                {
                    var move = (Move)Move.Path.DefaultFile.Copy();
                    displaceable.Activate(store.initialDirection, move);
                    ev.propagate = false;
                }
            }

            // bumped into something or couldn't move at all
            if (prevPos == ev.actor.Pos)
            {
                Status.Tinker.Untink(ev.actor);
            }
        }

        private static void SlideIfActionNull(ActorEvent ev)
        {
            var nextAction = ev.actor.Behaviors.TryGet<Acting>()?.NextAction;

            if (nextAction == null // if it were null, no action was tried

                // if it weren't one of these, no sliding has been done either
                || !nextAction.ContainsAction(typeof(BehaviorAction<Attacking>))
                || !nextAction.ContainsAction(typeof(BehaviorAction<Digging>))
                || !nextAction.ContainsAction(typeof(BehaviorAction<Moving>)))
            {
                SlideInstead(ev);
                ev.propagate = true;
            }

            else if (Status.Tinker.IsTinked(ev.actor) && !Sliding.IsWayFree(ev.actor))
            {
                Status.Tinker.Untink(ev.actor);
            }
        }

        private static ChainDefBuilder builder = new ChainDefBuilder()
            .AddDef(Attacking.Do)
            .AddHandler(SlideInstead, PriorityRanks.High)
            .AddDef(Digging.Do)
            .AddHandler(SlideInstead, PriorityRanks.High)
            .AddDef(Moving.Do)
            .AddHandler(SlideInstead, PriorityRanks.High)
            .AddDef(Tick.Chain)
            .AddHandler(SlideIfActionNull, PriorityRanks.High)
            .End();

        public static SlideStatus Status = new SlideStatus(1);
    }
}