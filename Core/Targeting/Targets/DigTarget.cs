using Core.Behaviors;
using Core.Stats.Basic;

namespace Core.Targeting
{
    public class DigTargetEvent : TargetEvent<DigTarget>
    {
        public Dig dig;

        public DigTargetEvent(Digging.Event ev) : base(ev)
        {
            dig = ev.dig;
        }
    }

    public class DigTarget : Target, ITarget<DigTarget, DigTargetEvent>
    {
        public Layer TargetedLayer => Layer.WALL;
        public Layer SkipLayer => 0;

        public void CalculateTargetedEntity(DigTargetEvent ev, Cell cell)
        {
            targetEntity = GetEntityDefault(cell, SkipLayer, TargetedLayer);
        }
    }
}