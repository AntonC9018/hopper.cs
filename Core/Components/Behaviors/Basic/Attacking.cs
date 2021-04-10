using System.Collections.Generic;
using Hopper.Core.Items;
using Hopper.Core.Targeting;
using Hopper.Core.Stats.Basic;
using System.Linq;
using Hopper.Utils.Vector;
using Hopper.Core.Predictions;
using Hopper.Core.Stats;
using Hopper.Shared.Attributes;

namespace Hopper.Core.Components.Basic
{
    [AutoActivation("Attack")]
    public partial class Attacking : IBehavior, IStandartActivateable, IBehaviorPredictable
    {
        public class Context : StandartContext
        {
            public List<Target> targets;
            [Omit] public Attack attack;
            [Omit] public Push push;
        }

        public bool Activate(Entity entity, IntVector2 direction)
        {
            return Activate(entity, direction, null);
        }

        /// <summary>
        /// A helper method. Tries to apply the given attack to the given entity, without checking the attackness. 
        /// </summary>
        // [Alias("TryApplyAttack")] 
        public static bool TryApplyAttack(Entity actor, Entity to, Attack attack, IntVector2 direction)
        {
            if (!to.IsDead())
            {
                return to.TryBeAttacked(actor, attack, direction);
            }
            return false;
        }

        /// <summary>
        /// This is one of the default handlers of the CHECK chain.
        /// It loads the attack and push from the stats manager.  
        /// </summary>
        [Export] public static void SetStats(
            Stats.Stats stats, ref Attack attack, ref Push push)
        {
            if (attack == null)
            {
                attack = stats.GetLazy(Attack.Path);
            }
            if (push == null)
            {
                push = stats.GetLazy(Push.Path);
            }
            // priority = PriorityMapping.Low + 0x8000
        }

        /// <summary>
        /// This is one of the default handlers of the CHECK chain.
        /// It finds targets using the target provider of the currently equipped weapon, if the entity has an inventory.
        /// Otherwise, the default target provider is used.
        /// @Rethink this should really be two separate handlers: one for entities with inventory and one for ones without one. There's no point in being extra abstract.
        /// </summary>
        [Export] public static void SetTargets(Context ctx)
        {
            if (ctx.targets == null)
            {
                if (ctx.actor.TryGetInventory(out var inventory))
                {
                    if (inventory.GetWeapon(out var weapon))
                    {
                        // Get targets from weapon, using its target provider
                        // @Incomplete: Save these initial targets at history or something
                        // since we don't want our context to have excessive data. 
                        // It may be useful in some cases, but not currenlty
                        ctx.targets = weapon
                            .GetTargets(ctx.actor, ctx.direction)
                            .ConvertAll(t => new Target(t.entity, t.piece.dir));
                    }
                    else
                    {
                        ctx.targets = new List<Target>();
                    }
                }
                else
                {
                    ctx.targets = TargetProvider.SimpleAttack.GetTargets(ctx.actor, ctx.direction).ToList();
                }
            }
            // priority = PriorityMapping.Low + 0x2000,
        }

        /// <summary>
        /// This is one of the default handlers of the DO chain.
        /// It tries to ATTACK all the targets one by one.
        /// </summary>
        [Export] public static void ApplyAttacks(
            List<Target> targets, Attack attack, Entity actor)
        {
            foreach (var target in targets)
            {
                TryApplyAttack(target.entity, actor, (Attack)attack.Copy(), target.direction);
            }
        }

        /// <summary>
        /// This is one of the default handlers of the DO chain.
        /// It tries to PUSH all the targets one by one.
        /// </summary>
        [Export] public static void ApplyPushes(List<Target> targets, Push push)
        {
            foreach (var target in targets)
            {
                target.entity.TryBePushed((Push)push.Copy(), target.direction);
            }
        }

        /// <summary>
        /// Returns the positions that would be attacked if the behavior's activate were to be called. 
        /// </summary>
        public IEnumerable<IntVector2> Predict(IntVector2 direction)
        {
            if (m_entity.Inventory != null)
            {
                if (m_entity.Inventory.GetWeapon(out var weapon))
                {
                    foreach (var relativePos in weapon.Pattern.Predict(direction))
                    {
                        yield return m_entity.Pos + relativePos;
                    }
                }
            }
            else
            {
                yield return m_entity.Pos + direction;
            }
        }

        // Check { SetTargets, SetStats }
        // Do    { ApplyAttack, ApplyPush, UpdateHistory }
    }
}