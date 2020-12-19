namespace Hopper.Core.Stats.Basic
{
    public class Health : StatFile
    {
        public int amount;

        public static readonly SimpleStatPath<Health> Path =
            new SimpleStatPath<Health>("health", new Health { amount = 1 });
    }
}