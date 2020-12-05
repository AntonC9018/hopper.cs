using System.Collections.Generic;
using Hopper.Core.Utils.Vector;
using Chains;

namespace Hopper.Core.Targeting
{
    public class TargetEvent<T> : EventBase
    {
        public List<T> targets;
        public IntVector2 dir;
        public IWorldSpot spot;
    }
}