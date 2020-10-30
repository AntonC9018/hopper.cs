using Core;
using Core.Stats;

namespace Test
{
    public class FreezeStat : StatusFile
    {
        public static readonly StatPath<FreezeStat> Path = new StatPath<FreezeStat>(
            "status/freeze", new FreezeStat { power = 1, amount = 3 });
    }
}