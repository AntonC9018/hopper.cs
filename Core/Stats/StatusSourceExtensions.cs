using Hopper.Core.Stat;

namespace Hopper.Core
{
    public static class StatusSourceExtensions
    {
        public static bool CheckResistance(this StatusSource source, Entity entityTheStatIsBeingAppliedTo, int powerOfStatOfApplier)
        {
            if (entityTheStatIsBeingAppliedTo.TryGetStats(out var stats))
            {
                stats.GetLazy(source.Index, out var resistance);
                if (resistance.amount >= powerOfStatOfApplier)
                {
                    return true;
                }
            }
            return false;
        }
    }
}