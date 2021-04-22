using System.Linq;
using Hopper.Core.Components.Basic;
using Hopper.Shared.Attributes;

namespace Hopper.Core.Retouchers
{
    public static partial class Skip
    {
        [Export(Chain = "Attacking.Check", Priority = PriorityRank.Low, Dynamic = true)]
        private static bool SkipEmptyAttack(Attacking.Context ctx)
        {
            return ctx.targetingContext.targetContexts.Count > 0;
        }

        [Export(Chain = "Digging.Check", Priority = PriorityRank.Low, Dynamic = true)]
        private static bool SkipEmptyDig(Digging.Context ctx)
        {
            return ctx.targets.Count > 0;
        }

        [Export(Chain = "Moving.Check", Priority = PriorityRank.Low, Dynamic = true)]
        private static bool SkipBlocked(Moving.Context ctx)
        {
            return !ctx.actor.GetTransform().HasBlockRelative(ctx.direction);
        }

        [Export(Chain = "Attacking.Check", Priority = PriorityRank.Low, Dynamic = true)]
        private static bool SkipNoPlayer(Attacking.Context ctx)
        {
            return ctx.targetingContext.targetContexts.Any(t => t.transform.entity.IsPlayer());
        }

        [Export(Chain = "Attacking.Check", Priority = PriorityRank.Low, Dynamic = true)]
        private static bool SkipSelf(Attacking.Context ctx)
        {
            return ctx.targetingContext.targetContexts.Any(t => t.transform.entity == ctx.actor);
        }
    }
}