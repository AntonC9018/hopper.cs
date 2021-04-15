using Hopper.Core.Components.Basic;

namespace Hopper.Core
{
    static partial class Algos
    {
        public static void StepBased(Acting.Context ctx)
        {
            ctx.actor.GetSequential().ApplyCurrentAlgo(ctx);
        }
    }
}