using Hopper.Shared.Attributes;
using Hopper.Utils.Chains;
using System.Collections.Generic;
using Hopper.Core.Items;
using Hopper.Core.Targeting;
using Hopper.Core.Stat.Basic;
using Hopper.Utils.Vector;
using Hopper.Core.Stat;
using System.Linq;

namespace Hopper.Core.Components.Basic
{
    [AutoActivation("Dig")]
    public partial class Digging : IBehavior, IStandartActivateable
    {
        public class Context : StandartContext
        {
            [Omit] public Dig dig;
            [Omit] public List<TargetContext> targets;
        }

        [Export] public static void SetDig(Stats stats, out Dig dig)
        {
            stats.GetLazy(Dig.Index, out dig);
        }

        [Export] public static void SetTargets(Context ctx)
        {
            if (ctx.targets == null
                && ctx.actor.TryGetInventory(out var inv)
                && inv.TryGetShovel(out var shovel)
                && shovel.TryGetUnbufferedTargetProvider(out var provider))
            {
                ctx.targets = provider.GetTargets(ctx.actor.GetTransform().position, ctx.direction).ToList();
            }
        }

        [Export] public static void Attack(Context ctx)
        {
            foreach (var target in ctx.targets)
            {
                ctx.dig.ToAttack(out Attack attack);
                target.transform.entity.TryBeAttacked(ctx.actor, attack, ctx.direction);
            }
        }

        public void DefaultPreset()
        {
            _CheckChain.Add(SetDigHandler, SetTargetsHandler);
            _DoChain.Add(AttackHandler);
        }
    }
}