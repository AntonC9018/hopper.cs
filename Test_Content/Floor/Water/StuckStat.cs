using Hopper.Core;
using Hopper.Core.Stats;

namespace Hopper.Test_Content.Floor
{
    public class StuckStat : StatusFile
    {
        public static readonly SimpleStatPath<StuckStat> Path = new SimpleStatPath<StuckStat>(
            "status/stuck", new StuckStat { power = 1, amount = 1 });
    }
}