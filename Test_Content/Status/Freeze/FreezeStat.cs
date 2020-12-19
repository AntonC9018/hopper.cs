using Hopper.Core;
using Hopper.Core.Stats;

namespace Hopper.Test_Content.Status.Freezing
{
    public class Freeze : StatusFile
    {
        public static readonly SimpleStatPath<Freeze> Path = new SimpleStatPath<Freeze>(
            "status/freeze", new Freeze { power = 1, amount = 3 });
        public static readonly FreezeStatus Status = new FreezeStatus(1);
    }
}