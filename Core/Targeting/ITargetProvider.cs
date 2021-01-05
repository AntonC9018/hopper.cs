using System.Collections.Generic;
using Hopper.Core.Predictions;
using Hopper.Utils.Vector;

namespace Hopper.Core.Targeting
{
    public interface ITargetProvider : IWithPattern
    {
        IEnumerable<Target> GetTargets(IWorldSpot spot, IntVector2 dir);
    }
}