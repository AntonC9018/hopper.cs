namespace Hopper.Core.Items
{
    public class ThrowawayPool : ISuperPool
    {
        public ISuperPool Copy()
        {
            return this;
        }

        public PoolItem GetNextItem(string path)
        {
            return null;
        }
    }
}