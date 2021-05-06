using Hopper.Core.Stat;

namespace Hopper.Core
{
    public static class StatusSourceExtensions
    {
        public static bool CanResist(this Entity entityTheStatIsBeingAppliedTo, StatusSource source, int powerOfStatOfApplier)
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

        public static bool CanNotResist(this Entity entityTheStatIsBeingAppliedTo, StatusSource source, int powerOfStatOfApplier)
        {
            return !CanResist(entityTheStatIsBeingAppliedTo, source, powerOfStatOfApplier);
        }
    }
}