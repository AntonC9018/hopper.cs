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

        private bool HasIceUnder(Entity entity)
        {
            var floor = entity.Cell.GetEntityFromLayer(Layer.FLOOR);
            return floor != null && floor.Behaviors.Has<Sliding>();
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

        private static void SlideInstead(ActorEvent ev)
        {
            var store = Status.Tinker.GetStore(ev.actor);
            var displaceable = ev.actor.Behaviors.TryGet<Displaceable>();
            var move = (Move)Move.Path.DefaultFile.Copy();
            var prevPos = ev.actor.Pos;
            displaceable?.Activate(store.initialDirection, move);

            // bumped into something
            if (prevPos == ev.actor.Pos)
            {
                store.amount = 0;
            }

            ev.propagate = false;
        }

        private static void SlideIfActionNull(ActorEvent ev)
        {
            if (ev.actor.Behaviors.TryGet<Acting>()?.NextAction == null)
            {
                SlideInstead(ev);
                ev.propagate = true;
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