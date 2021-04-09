using Hopper.Core.Registries;
using System.Linq;
using Hopper.Core.Components.Basic;
using Hopper.Utils.Chains;
using Hopper.Core.Components;

namespace Hopper.Core.Retouchers
{
    public static class Skip
    {
        [Export]
        private static bool IsAttackEmpty(Attacking.Context ctx)
        {
            return ctx.targets.Count > 0;
        }

        private static bool IsDigEmpty(Digging.Context ctx)
        {
            return ctx.targets.Count > 0;
        }

        private static void SkipBlocked(Moving.Context ctx)
        {
            ctx.propagate = ctx.actor.HasBlockRelative(ctx.direction) == false;
        }

        private static void SkipNoPlayer(Attacking.Context ctx)
        {
            ctx.propagate = ctx.targets.Any(t => t.entity.IsPlayer());
        }

        private static void SkipSelf(Attacking.Context ctx)
        {
            ctx.propagate = ctx.targets
                .Any(t => t.entity == ctx.actor);
        }
    }
}