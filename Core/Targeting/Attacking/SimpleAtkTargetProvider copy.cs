using System.Collections.Generic;
using Hopper.Core.Components.Basic;
using Hopper.Utils.Vector;

namespace Hopper.Core.Targeting
{
    public class SingleAtkTargetProvider : ITargetProvider
    {
        public IPattern Pattern => Targeting.Pattern.Default;
        private Layer m_skipLayer;
        private Layer m_targetLayer;

        public SingleAtkTargetProvider(Layer skipLayer, Layer targetLayer)
        {
            m_skipLayer = skipLayer;
            m_targetLayer = targetLayer;
        }

        public IEnumerable<Target> GetTargets(IWorldSpot spot, IntVector2 direction)
        {
            Cell cell = spot.GetCellRelative(direction);
            if (cell != null && cell.HasBlock(direction, m_skipLayer) == false)
            {
                Entity entity = cell.GetEntityFromLayer(direction, m_targetLayer);
                if (entity != null
                    && entity.Behaviors.Has<Attackable>()
                    && entity.Behaviors.Get<Attackable>().IsAttackable(spot))
                {
                    yield return new Target(entity, direction);
                }
            }
        }
    }
}