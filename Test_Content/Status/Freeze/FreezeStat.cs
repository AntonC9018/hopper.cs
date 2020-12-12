using Hopper.Core;
using Hopper.Core.Stats;

namespace Hopper.Test_Content.Status.Freeze
{
    public class FreezeStat : StatusFile
    {
        public static readonly SimpleStatPath<FreezeStat> Path = new SimpleStatPath<FreezeStat>(
            "status/freeze", new FreezeStat { power = 1, amount = 3 });
    }
}