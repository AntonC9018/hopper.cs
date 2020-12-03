using System.Collections.Generic;
using Core.Behaviors;
using Core.Stats.Basic;
using Core.Utils.Vector;

namespace Core.Targeting
{
    public class MultiAtkTargetProvider : IAtkTargetProvider
    {
        private IPattern m_pattern;
        private Layer m_skipLayer;
        private Layer m_targetLayer;

        public MultiAtkTargetProvider(
            IPattern pattern,
            Layer skipLayer,
            Layer targetLayer)
        {
            m_pattern = pattern;
            m_skipLayer = skipLayer;
            m_targetLayer = targetLayer;
        }

        public IEnumerable<AtkTarget> GetTargets(IWorldSpot spot, IntVector2 dir, Attack attack)
        {
            var targets = new List<AtkTarget>();

            foreach (var rotatedPiece in m_pattern.GetPieces(spot, dir))
            {
                Cell cell = spot.GetCellRelative(rotatedPiece.pos);
                if (cell != null && cell.HasBlock(dir, m_skipLayer) == false)
                {
                    var entities = cell.GetAllFromLayer(dir, m_targetLayer);
                    foreach (var entity in entities)
                    {
                        if (entity.Behaviors.Has<Attackable>())
                        {
                            var atkness = entity.Behaviors.Get<Attackable>().GetAtkCondition(attack);
                            if (atkness == AtkCondition.NEVER || atkness == AtkCondition.SKIP)
                            {
                                var target = new AtkTarget(entity, dir);
                                target.atkCondition = atkness;
                                targets.Add(target);
                            }
                        }
                    }
                }
            }

            return targets;
        }
    }
}