using Hopper.Utils;

namespace Hopper.Core.Targeting
{
    public struct Reach
    {
        public int[] values;
        public bool reachesAll => values == null;

        public Reach(params int[] values)
        {
            this.values = values;
        }

        public Reach(bool alwaysReach)
        {
            Assert.That(alwaysReach);
            this.values = null;
        }
    }
}