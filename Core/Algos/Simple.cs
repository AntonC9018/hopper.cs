using Hopper.Core.Components.Basic;

namespace Hopper.Core
{
    static partial class Algos
    {
        public static void SimpleAlgo(Acting.Context ctx)
        {
            ctx.success = ctx.action.Do(ctx.acting);
        }

    }
}