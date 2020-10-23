using Core.Stats.Basic;

namespace Core.Targeting
{

    public class DigTarget : Target, ITarget<DigTarget, Dig>
    {
        public void CalculateTargetedEntity(
            TargetEvent<DigTarget> ev, Cell cell, Layer skipLayer, Layer targetedLayer)
        {
            targetEntity = GetEntityDefault(cell, skipLayer, targetedLayer);
        }

        public void ProcessMeta(Dig meta)
        {
        }
    }
}