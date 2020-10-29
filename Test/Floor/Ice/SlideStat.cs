using Core;
using Core.Stats;

namespace Test
{
    public class SlideStat : StatusFile
    {
        public static readonly StatPath<SlideStat> Path = new StatPath<SlideStat>(
            "status/slide", new SlideStat { power = 1, amount = 1 });
    }
}