using System.Collections.Generic;
using Hopper.Core.Behaviors.Basic;
using Hopper.Utils.Vector;

namespace Hopper.Core.Targeting
{
    public class MultiAtkTargetProvider : ITargetProvider
    {
        private IPattern m_pattern;
        private TargetLayers m_targetLayers;

        public MultiAtkTargetProvider(
            IPattern pattern,
            TargetLayers layers)
        {
            m_pattern = pattern;
            m_targetLayers = layers;
        }

        public IEnumerable<Target> GetTargets(IWorldSpot spot, IntVector2 direction)
        {
            foreach (var rotatedPiece in m_pattern.GetPieces(spot, direction))
            {
                Cell cell = spot.GetCellRelative(rotatedPiece.pos);
                if (cell != null && cell.HasBlock(direction, m_targetLayers.skip) == false)
                {
                    var entities = cell.GetAllFromLayer(direction, m_targetLayers.targeted);
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