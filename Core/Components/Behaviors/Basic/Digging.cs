using Hopper.Shared.Attributes;
using Hopper.Utils.Chains;
using System.Collections.Generic;
using Hopper.Core.Items;
using Hopper.Core.Targeting;
using Hopper.Core.Stats.Basic;
using Hopper.Utils.Vector;
using Hopper.Core.Stats;

namespace Hopper.Core.Components.Basic
{
    [AutoActivation("Dig")]
    public partial class Digging : IBehavior, IStandartActivateable
    {
        public class Context : StandartContext
        {
            [Omit] public Dig dig;
            [Omit] public List<Target> targets;
        }

        [Export] public static void SetDig(Stats.Stats stats, out Dig dig)
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

        public void DefaultPreset()
        {
            _CheckChain.Add(SetDigHandler, SetTargetsHandler);
            _DoChain.Add(AttackHandler);
        }
    }
}