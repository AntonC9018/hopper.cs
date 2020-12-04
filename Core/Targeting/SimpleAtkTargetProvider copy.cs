using System.Collections.Generic;
using Chains;
using Core.Behaviors;
using Core.Stats.Basic;
using Core.Utils.Vector;

namespace Core.Targeting
{
    public class SimpleAtkTargetProvider : IAtkTargetProvider
    {
        private Layer m_skipLayer;
        private Layer m_targetLayer;

        public SimpleAtkTargetProvider(Layer skipLayer, Layer targetLayer)
        {
            m_skipLayer = skipLayer;
            m_targetLayer = targetLayer;
        }

        public IEnumerable<Target> GetTargets(IWorldSpot spot, IntVector2 dir, Attack attack)
        {
            Cell cell = spot.GetCellRelative(dir);
            if (cell != null && cell.HasBlock(dir, m_skipLayer) == false)
            {
                var entity = cell.GetEntityFromLayer(dir, Layer.WALL);
                if (entity != null && entity.Behaviors.Has<Attackable>())
                {
                    var atkness = entity.Behaviors.Get<Attackable>().GetAtkCondition(attack);
                    if (atkness != AtkCondition.SKIP && atkness != AtkCondition.NEVER)
                    {
                        yield return new Target(entity, dir);
                    }
                }
            }
        }
    }
}