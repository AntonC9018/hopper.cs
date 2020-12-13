using System.Collections.Generic;
using Hopper.Utils.Vector;

namespace Hopper.Core.Targeting
{
    public interface ITargetProvider
    {
        IEnumerable<Target> GetTargets(IWorldSpot spot, IntVector2 dir);
    }
}