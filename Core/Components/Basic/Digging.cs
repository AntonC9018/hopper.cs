using Hopper.Shared.Attributes;
using Hopper.Utils.Chains;
using System.Collections.Generic;
using Hopper.Core.Items;
using Hopper.Core.Targeting;
using Hopper.Core.Stat;
using System.Linq;
using Hopper.Core.ActingNS;
using Hopper.Core.WorldNS;
using Hopper.Utils.Vector;

namespace Hopper.Core.Components.Basic
{
    public partial class Digging : IBehavior, IStandartActivateable
    {
        public class Context : StandartContext
        {
        }

        // TODO: Currently unused
        [Chain("After")] private readonly Chain<Context> _AfterChain;
        public bool Activate(Entity actor, IntVector2 direction)
        {
            if (actor.TryGetInventory(out var inv)
                && inv.TryGetShovel(out var shovel)
                && shovel.TryGetUnbufferedTargetProvider(out var provider))
            {
                var targets = provider.GetTargets(actor.GetTransform().position, direction);

                if (!targets.Any()) return false;

                var stats = actor.GetStats();
                var dig = stats.GetLazy(Dig.Index);
                var attack = dig.ToAttack();
                
                foreach (var target in targets)
                {
                    target.transform.entity.TryBeAttacked(actor, attack, direction);
                }

                return true;
            }

            return false;
        }
    }
}