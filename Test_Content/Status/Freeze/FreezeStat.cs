using Hopper.Core;
using Hopper.Core.Stats;

namespace Hopper.Test_Content
{
    public class FreezeStat : StatusFile
    {
        public static readonly StatPath<FreezeStat> Path = new StatPath<FreezeStat>(
            "status/freeze", new FreezeStat { power = 1, amount = 3 });
    }
}