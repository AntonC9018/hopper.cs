using Hopper.Shared.Attributes;
using Hopper.Utils.Chains;
using System.Collections.Generic;
using Hopper.Core.Items;
using Hopper.Core.Targeting;
using Hopper.Core.Stat.Basic;
using Hopper.Utils.Vector;
using Hopper.Core.Stat;

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

        [Export] public static void SetDig(Stats stats, out Dig dig)
        {
            stats.GetLazy(Dig.Index, out dig);
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
                ctx.dig.ToAttack(out Attack attack);
                target.entity.TryBeAttacked(ctx.actor, attack, ctx.direction);
            }
        }

        public void DefaultPreset()
        {
            _CheckChain.Add(SetDigHandler, SetTargetsHandler);
            _DoChain.Add(AttackHandler);
        }
    }
}