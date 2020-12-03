using System.Collections.Generic;
using Core.Stats.Basic;
using Core.Utils.Vector;

namespace Core.Targeting
{
    public interface IAtkTargetProvider
    {
        IEnumerable<AtkTarget> GetTargets(IWorldSpot spot, IntVector2 dir, Attack attack);
    }
}