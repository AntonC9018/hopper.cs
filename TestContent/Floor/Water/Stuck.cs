using Hopper.Core;
using Hopper.Core.Stat;

namespace Hopper.TestContent.Floor
{
    public class Stuck : StatusFile
    {
        public static readonly SimpleStatPath<Stuck> Path = new SimpleStatPath<Stuck>(
            "status/stuck", new Stuck { power = 1, amount = 1 });
        public static readonly StuckStatus Status = StuckStatus.Create(1);
    }
}