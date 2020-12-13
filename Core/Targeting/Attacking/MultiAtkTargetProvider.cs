using System.Collections.Generic;
using Hopper.Core.Behaviors.Basic;
using Hopper.Core.Stats.Basic;
using Hopper.Utils.Vector;

namespace Hopper.Core.Targeting
{
    public class MultiTargetProvider : IAtkTargetProvider
    {
        private IPattern m_pattern;
        private Layer m_skipLayer;
        private Layer m_targetLayer;

        public MultiTargetProvider(
            IPattern pattern,
            Layer skipLayer,
            Layer targetLayer)
        {
            m_pattern = pattern;
            m_skipLayer = skipLayer;
            m_targetLayer = targetLayer;
        }

        public IEnumerable<Target> GetTargets(IWorldSpot spot, IntVector2 direction)
        {
            foreach (var rotatedPiece in m_pattern.GetPieces(spot, direction))
            {
                Cell cell = spot.GetCellRelative(rotatedPiece.pos);
                if (cell != null && cell.HasBlock(direction, m_skipLayer) == false)
                {
                    var entities = cell.GetAllFromLayer(direction, m_targetLayer);
                    foreach (var entity in entities)
                    {
                        if (entity.Behaviors.Has<Attackable>())
                        {
                            // we can disregard the attackableness, since it will be blocked anyway.
                            // var atkness = entity.Behaviors.Get<Attackable>().GetAtkCondition(attack);
                            // if (atkness == AtkCondition.NEVER || atkness == AtkCondition.SKIP)
                            yield return new Target(entity, direction);
                        }
                    }
                }
            }
        }
    }
}