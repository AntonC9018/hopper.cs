using Hopper.Core;
using Hopper.Core.Stats;

namespace Hopper.Test_Content
{
    public class StuckStat : StatusFile
    {
        public static readonly StatPath<StuckStat> Path = new StatPath<StuckStat>(
            "status/stuck", new StuckStat { power = 1, amount = 1 });
    }
}