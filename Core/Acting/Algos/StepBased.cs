using Hopper.Core.ActingNS;

namespace Hopper.Core.ActingNS
{
    static partial class Algos
    {
        public static void StepBased(Acting.Context ctx)
        {
            ctx.actor.GetSequential().ApplyCurrentAlgo(ctx);
        }
    }
}