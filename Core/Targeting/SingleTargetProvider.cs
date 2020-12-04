using System.Collections.Generic;
using Core.Utils.Vector;

namespace Core.Targeting
{
    public class SingleTargetProvider : ITargetProvider<Target>
    {
        private Layer m_skipLayer;
        private Layer m_targetLayer;

        public SingleTargetProvider(Layer skipLayer, Layer targetLayer)
        {
            m_skipLayer = skipLayer;
            m_targetLayer = targetLayer;
        }

        public IEnumerable<Target> GetTargets(IWorldSpot spot, IntVector2 dir)
        {
            Cell cell = spot.GetCellRelative(dir);
            if (cell != null && cell.HasBlock(dir, m_skipLayer) == false)
            {
                var entity = cell.GetEntityFromLayer(dir, Layer.WALL);
                if (entity != null)
                {
                    yield return new Target(entity, dir);
                }
            }
        }
    }
}