using System.Collections.Generic;
using Chains;
using Core.Behaviors;
using Core.Stats.Basic;
using Core.Utils.Vector;

namespace Core.Targeting
{
    public class SimpleDigTargetProvider : ITargetProvider<Target>
    {
        public IEnumerable<Target> GetTargets(IWorldSpot spot, IntVector2 dir)
        {
            var targets = new List<Target>();

            Cell cell = spot.GetCellRelative(dir);
            var entity = cell.GetEntityFromLayer(dir, Layer.WALL);
            if (entity != null)
            {
                targets.Add(new Target(entity, dir));
            }

            return targets;
        }
    }
}