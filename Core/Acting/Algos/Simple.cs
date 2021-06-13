using Hopper.Core.ActingNS;

namespace Hopper.Core.ActingNS
{
    static partial class Algos
    {
        public static void SimpleAlgo(Acting.Context ctx)
        {
            ctx.Success = ctx.action.DoAction(ctx.actor);
        }

    }
}