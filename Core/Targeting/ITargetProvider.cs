using System.Collections.Generic;
using Hopper.Utils.Vector;

namespace Hopper.Core.Targeting
{
    public interface ITargetProvider<T> where T : Target
    {
        IEnumerable<T> GetTargets(IWorldSpot spot, IntVector2 dir);
    }
}