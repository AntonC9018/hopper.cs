using System.Collections.Generic;
using Utils.Vector;
using Chains;

namespace Core.Targeting
{
    public class TargetEvent<T> : EventBase where T : Target
    {
        public List<T> targets;
        public IntVector2 dir;
        public IWorldSpot spot;
    }
}