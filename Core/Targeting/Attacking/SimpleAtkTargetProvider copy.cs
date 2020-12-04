using System.Collections.Generic;
using Core.Behaviors;
using Core.Stats.Basic;
using Core.Utils.Vector;

namespace Core.Targeting
{
    public class SingleAtkTargetProvider : IAtkTargetProvider
    {
        private Layer m_skipLayer;
        private Layer m_targetLayer;

        public SingleAtkTargetProvider(Layer skipLayer, Layer targetLayer)
        {
            m_skipLayer = skipLayer;
            m_targetLayer = targetLayer;
        }

        public IEnumerable<Target> GetTargets(IWorldSpot spot, IntVector2 dir, Attack attack)
        {
            Cell cell = spot.GetCellRelative(dir);
            if (cell != null && cell.HasBlock(dir, m_skipLayer) == false)
            {
                Entity entity = cell.GetEntityFromLayer(dir, Layer.WALL);
                if (entity != null
                    && entity.Behaviors.Has<Attackable>()
                    && entity.Behaviors.Get<Attackable>().IsAttackable(attack, spot))
                {
                    yield return new Target(entity, dir);
                }
            }
        }
    }
}