using Hopper.Utils.Chains;
using System.Collections.Generic;
using Hopper.Core.Items;
using Hopper.Core.Targeting;
using Hopper.Core.Stats.Basic;
using Hopper.Core.Chains;
using Hopper.Utils.Vector;
using Hopper.Core.Stats;

namespace Hopper.Core.Components.Basic
{
    [AutoActivation("Dig")]
    public class Digging : IBehavior, IStandartActivateable
    {
        public class Context : StandartEvent
        {
            public Dig dig = null;
            public List<Target> targets = null;
        }

        [Export] public static void SetDig(StatManager stats, out Dig dig)
        {
            dig = stats.GetLazy(Dig.Path);
        }

        [Export] public static void SetTargets(Context ctx)
        {
            if (ctx.targets == null)
            {
                ctx.targets = new List<Target>();
                if (ctx.actor.TryGetInventory(out var inv) && inv.TryGetShovel(out var shovel))
                {
                    ctx.targets.AddRange(shovel.GetTargets(ctx.actor, ctx.direction));
                }
            }
        }

        [Export] public static void Attack(Context ctx)
        {
            foreach (var target in ctx.targets)
            {
                target.entity.TryBeAttacked(ctx.actor, ctx.dig.ToAttack(), ctx.direction);
            }
        }

        // Check { SetDig, SetTargets }
        // Do    { Attack }
    }
}