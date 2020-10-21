using System.Collections.Generic;
using Utils;
using Utils.Vector;

namespace Core.Targeting
{
    public class MultiTarget<T, E> : Target, ITarget<T, E>
        where T : Target, new()
        where E : TargetEvent<T>
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

        public IEnumerable<T> CalculateTargets(E targetEvent, Cell cell)
        {
            var targetedEntities = cell.m_entities.Where(
                e => (e.Layer & SkipLayer) == 0
                    && (e.Layer & TargetedLayer) != 0
            );
            return ToTargets(targetedEntities);
        }

        public void CalculateTargetedEntity(E ev, Cell cell)
        {
            throw new System.NotImplementedException();
        }
    }
}