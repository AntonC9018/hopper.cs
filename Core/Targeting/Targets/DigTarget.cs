using Core.Stats.Basic;
using Utils.Vector;

namespace Core.Targeting
{

    public class DigTarget : Target, ITarget<DigTarget, Dig>
    {

        public Layer TargetedLayer => Layer.WALL;
        public Layer SkipLayer => 0;

        public void CalculateTargetedEntity(TargetEvent<DigTarget> ev, Cell cell, Dig dig)
        {
            targetEntity = GetEntityDefault(cell, SkipLayer, TargetedLayer);
        }
    }
}