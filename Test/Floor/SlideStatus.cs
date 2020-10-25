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

        public override bool Update(Entity entity)
        {
            if (entity.Tinkers.IsTinked(this))
            {
                var store = GetStore(entity);

                if (HasIceUnder(entity) == false)
                {
                    Untink(entity);
                    store.amount = 0;
                }

                return store.amount == 0;
            }
            return true;
        }

        private static void SlideInstead(ActorEvent ev)
        {
            var store = Status.GetStore(ev.actor);
            var displaceable = ev.actor.Behaviors.Get<Displaceable>();
            var move = (Move)Move.Path.DefaultFile.Copy();
            var prevPos = ev.actor.Pos;
            displaceable?.Activate(store.initialDirection, move);

            // bumped into something
            if (prevPos.Equals(ev.actor.Pos))
            {
                store.amount = 0;
            }

            ev.propagate = false;
        }

        private static ChainDefBuilder builder = new ChainDefBuilder()
            .AddDef(Attacking.Do)
            .AddHandler(SlideInstead, PriorityRanks.High)
            .AddDef(Digging.Do)
            .AddHandler(SlideInstead, PriorityRanks.High)
            .AddDef(Moving.Do)
            .AddHandler(SlideInstead, PriorityRanks.High)
            .End();

        public static SlideStatus Status = new SlideStatus(1);
    }
}