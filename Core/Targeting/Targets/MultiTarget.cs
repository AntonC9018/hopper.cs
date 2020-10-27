using System.Collections.Generic;
using Core.Utils;

namespace Core.Targeting
{
    public class MultiTarget<T, M> : Target //, ITarget<T, M>
        where T : Target, ITarget<T, M>, new()
    {
        public IEnumerable<T> ToTargets(IList<Entity> targetedEntities, M meta)
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
                result[i].ProcessMeta(meta);
            }
            return result;
        }

        public IEnumerable<T> CalculateTargets(
            TargetEvent<T> targetEvent,
            Cell cell,
            M meta,
            Layer skipLayer,
            Layer targetedLayer)
        {
            var targetedEntities = cell.m_entities
                .Where(e => (e.Layer & skipLayer) == 0 && (e.Layer & targetedLayer) != 0);
            return ToTargets(targetedEntities, meta);
        }
    }
}