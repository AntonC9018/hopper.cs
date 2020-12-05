using Hopper.Core;
using Hopper.Core.Stats;

namespace Hopper.Test_Content
{
    public class SlideStat : StatusFile
    {
        public static readonly StatPath<SlideStat> Path = new StatPath<SlideStat>(
            "status/slide", new SlideStat { power = 1, amount = 1 });
    }
}