using System.Collections.Generic;
using Core.Utils.Vector;
using Chains;

namespace Core.Targeting
{
    public class TargetEvent<T> : EventBase
    {
        public List<T> targets;
        public IntVector2 dir;
        public IWorldSpot spot;
    }
}