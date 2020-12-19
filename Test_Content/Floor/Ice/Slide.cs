using Hopper.Core;
using Hopper.Core.Stats;

namespace Hopper.Test_Content.Floor
{
    public class Slide : StatusFile
    {
        public static readonly SimpleStatPath<Slide> Path = new SimpleStatPath<Slide>(
            "status/slide", new Slide { power = 1, amount = 1 });
        public static readonly SlideStatus Status = SlideStatus.Create(1);

    }
}