using System.Collections.Generic;
using Utils;
using Utils.Vector;

namespace Core.Targeting
{
    public class MultiTarget<T> : Target where T : Target, new()
    {
        public IEnumerable<T> ToTargets(IList<Entity> targetedEntities)
        {
            T[] result = new T[targetedEntities.Count];
            for (int i = 0; i < targetedEntities.Count; i++)
            {
                result[i] = new T
                {
                    initialPiece = initialPiece,
                    direction = direction,
                    targetEntity = targetedEntities[i]
                };
            }
            return result;
        }

        public IEnumerable<T> CalculateTargets(CommonEvent ev, Cell cell)
        {
            var targetedEntities = cell.m_entities.Where(
                e => (e.Layer & SkipLayer) == 0
                    && (e.Layer & TargetedLayer) != 0
            );
            return ToTargets(targetedEntities);
        }
    }
}