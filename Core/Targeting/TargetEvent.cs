using System.Collections.Generic;
using Hopper.Utils.Vector;
using Hopper.Utils.Chains;

namespace Hopper.Core.Targeting
{
    public class TargetEvent<T> : ContextBase
    {
        public List<T> targets;
        public IntVector2 direction;
        public IWorldSpot spot;
    }
}