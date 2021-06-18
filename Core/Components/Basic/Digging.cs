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
            public IEnumerable<TargetContext> targets;

            public Context(Entity actor, IntVector2 direction, IEnumerable<TargetContext> targets)
            {
                this.actor     = actor;
                this.direction = direction;
                this.targets   = targets;
            }

            public void AttackTargets(Dig dig)
            {
                var attack = dig.ToAttack();
                foreach (var target in targets)
                {
                    target.transform.entity.TryBeAttacked(actor, attack, direction);
                }
            }
        }

        [Chain("After")] private readonly Chain<Context> _AfterChain;
        [Chain("Check")] private readonly Chain<Context> _CheckChain;

        public bool Activate(Entity actor, IntVector2 direction)
        {
            var context = new Context(actor, direction, GetTargetsShovel(actor, direction));
            
            if (!_CheckChain.PassWithPropagationChecking(context))
            {
                return false;
            }

            var dig = actor.GetStats().GetLazy(Dig.Index);
            context.AttackTargets(dig);

            _AfterChain.Pass(context);

            return true;
        }

        public static IEnumerable<TargetContext> GetTargetsShovel(Entity actor, IntVector2 direction)
        {
            if (actor.TryGetInventory(out var inv)
                && inv.TryGetShovel(out var shovel)
                && shovel.TryGetUnbufferedTargetProvider(out var provider))
            {
                return provider.GetTargets(actor.GetTransform().position, direction);
            }

            return Enumerable.Empty<TargetContext>();
        }
    }
}