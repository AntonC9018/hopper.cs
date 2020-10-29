using Core;
using Core.Stats;

namespace Test
{
    public class StuckStat : StatusFile
    {
        public static readonly StatPath<StuckStat> Path = new StatPath<StuckStat>(
            "status/stuck", new StuckStat { power = 1, amount = 1 });
    }
}