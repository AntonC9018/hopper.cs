using Hopper.Core.ActingNS;

namespace Hopper.Core.ActingNS
{
    static partial class Algos
    {
        public static void SimpleAlgo(Acting.Context ctx)
        {
            ctx.success = ctx.action.DoAction(ctx.actor);
        }

    }
}