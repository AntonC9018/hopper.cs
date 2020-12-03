using System.Collections.Generic;
using Core.Utils.Vector;

namespace Core.Targeting
{
    public interface ITargetProvider<T> where T : Target
    {
        IEnumerable<T> GetTargets(IWorldSpot spot, IntVector2 dir);
    }
}