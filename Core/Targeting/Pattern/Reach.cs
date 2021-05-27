using Hopper.Utils;

namespace Hopper.Core.Targeting
{
    public struct Reach
    {
        public int[] indices;
        public bool reachesAll => indices == null;

        public Reach(params int[] values)
        {
            this.indices = values;
        }

        public Reach(bool alwaysReach)
        {
            Assert.That(alwaysReach);
            this.indices = null;
        }
    }
}