using System.Collections.Generic;
using Utils;
using Utils.Vector;

namespace Core.Targeting
{
    public class MultiTarget<T, M> : Target, ITarget<T, M>
        where T : Target, ITarget<T, M>, new()
    {
        public virtual Layer TargetedLayer => throw new System.NotImplementedException();
        public virtual Layer SkipLayer => throw new System.NotImplementedException();

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

        public IEnumerable<T> CalculateTargets(TargetEvent<T> targetEvent, Cell cell, M meta)
        {
            var targetedEntities = cell.m_entities.Where(
                e => (e.Layer & SkipLayer) == 0
                    && (e.Layer & TargetedLayer) != 0
            );
            return ToTargets(targetedEntities);
        }

        public void CalculateTargetedEntity(TargetEvent<T> ev, Cell cell, M meta)
        {
            throw new System.NotImplementedException();
        }
    }
}