using Hopper.Core;
using Hopper.Core.Stats;

namespace Test
{
    public class FreezeStat : StatusFile
    {
        public static readonly StatPath<FreezeStat> Path = new StatPath<FreezeStat>(
            "status/freeze", new FreezeStat { power = 1, amount = 3 });
    }
}