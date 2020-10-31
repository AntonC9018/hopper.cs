using System.Collections.Generic;
using System.Linq;
using Core.Utils;

namespace Core.Targeting
{
    public class MultiTarget<T, M> : Target //, ITarget<T, M>
        where T : Target, ITarget<T, M>, new()
    {
        public IEnumerable<T> ToTargets(IEnumerable<Entity> targetedEntities, M meta)
        {
            foreach (var entity in targetedEntities)
            {
                var result = new T
                {
                    initialPiece = initialPiece,
                    direction = direction,
                    targetEntity = entity
                };
                result.ProcessMeta(meta);
                yield return result;
            }
        }

        public IEnumerable<T> CalculateTargets(
            TargetEvent<T> targetEvent,
            Cell cell,
            M meta,
            Layer skipLayer,
            Layer targetedLayer)
        {
            if (cell.HasBlock(targetEvent.dir, skipLayer) == false)
            {
                return new T[0];
            }
            return ToTargets(cell.GetAllFromLayer(targetEvent.dir, targetedLayer).ToList(), meta);
        }
    }
}