using System.Collections.Generic;
using Hopper.Core.Items;
using Hopper.Core.Targeting;
using Hopper.Core.Stat;
using System.Linq;
using Hopper.Utils.Vector;
using Hopper.Core.Predictions;
using Hopper.Shared.Attributes;
using System;

namespace Hopper.Core.Components.Basic
{
    [AutoActivation("Attack")]
    public partial class Attacking : IBehavior, IStandartActivateable, IBehaviorPredictable
    {
        public class Context : StandartContext
        {
            public AttackTargetingContext targetingContext;
            [Omit] public bool haveStatsBeenSet;
            [Omit] public Attack attack;
            [Omit] public Push push;

            public void SetSingleTarget(Transform target)
            {
                var transform = actor.GetTransform();

                targetingContext = new AttackTargetingContext(
                    new List<AttackTargetContext>(1) { new AttackTargetContext(transform) },
                    null, actor, transform.position, direction, target.layer, 0);
            }
        }

        public bool Activate(Entity entity, IntVector2 direction)
        {
            return Activate(entity, direction, null);
        }

        /// <summary>
        /// A helper method. Tries to apply the given attack to the given entity, without checking the attackness. 
        /// </summary>
        // [Alias("TryApplyAttack")] 
        public static bool TryApplyAttack(Entity attacker, Entity attacked, Attack attack, IntVector2 direction)
        {
            if (!attacked.IsDead())
            {
                return attacked.TryBeAttacked(attacker, attack, direction);
            }
            return false;
        }

        /// <summary>
        /// This is one of the default handlers of the CHECK chain.
        /// It loads the attack and push from the stats manager.  
        /// </summary>
        [Export(Priority = PriorityRank.Low)] public static void SetStats(Context ctx)
        {
            if (!ctx.haveStatsBeenSet)
            {
                var stats = ctx.actor.GetStats();
                stats.GetLazy(Attack.Index, out ctx.attack);
                stats.GetLazy(Push.Index, out ctx.push);
            }
        }

        /// <summary>
        /// This is one of the default handlers of the CHECK chain.
        /// It finds targets using the target provider of the currently equipped weapon, if the entity has an inventory.
        /// </summary>
        [Export(Priority = PriorityRank.Low)] public static void SetTargets(Context ctx)
        {
            if (
                ctx.targetingContext == null
                && ctx.actor.TryGetInventory(out var inventory)
                && inventory.TryGetWeapon(out var weapon)
                && weapon.TryGetWeaponTargetProvider(out var provider))
            {
                ctx.targetingContext = provider.GetTargets(ctx.actor, ctx.direction);
            }
        }

        /// <summary>
        /// This is one of the default handlers of the CHECK chain.
        /// It gets the entity that is right in the direction that this entity has decided to attack.
        /// Should be used for enemies without a weapon in the inventory.
        /// </summary>
        [Export(Priority = PriorityRank.Low)] public static void SetTargetsRightBeside(Context ctx)
        {
            if (ctx.targetingContext == null)
            {
                ctx.targetingContext = BufferedAttackTargetProvider.Simple.GetTargets(ctx.actor, ctx.direction);
            }
        }

        /// <summary>
        /// This is one of the default handlers of the DO chain.
        /// It tries to ATTACK all the targets one by one.
        /// </summary>
        [Export] public static void ApplyAttacks(
            AttackTargetingContext targetingContext, Attack attack, Entity actor)
        {
            foreach (var target in targetingContext.targetContexts)
            {
                TryApplyAttack(target.transform.entity, actor, attack.Copy(), target.direction);
            }
        }

        /// <summary>
        /// This is one of the default handlers of the DO chain.
        /// It tries to PUSH all the targets one by one.
        /// </summary>
        [Export] public static void ApplyPushes(AttackTargetingContext targetingContext, Push push)
        {
            foreach (var target in targetingContext.targetContexts)
            {
                target.transform.entity.TryBePushed(push.Copy(), target.direction);
            }
        }

        /// <summary>
        /// Returns the positions that would be attacked if the behavior's activate were to be called. 
        /// </summary>
        public IEnumerable<IntVector2> Predict(Acting acting, IntVector2 direction)
        {
            Entity actor = acting.actor;
            if (_DoChain.Contains(SetTargetsRightBesideHandler))
            {
                yield return actor.GetTransform().position + direction;
            }
            else if (actor.TryGetInventory(out var inventory)
                && inventory.TryGetWeapon(out var weapon)
                && weapon.TryGetWeaponTargetProvider(out var provider))
            {
                var pos = actor.GetTransform().position;

                foreach (var ctx in provider._pattern.MakeContexts(pos, direction))
                {
                    // This has more info, can use.
                    yield return ctx.position;
                }
            }
        }

        // Check { SetTargets, SetStats }
        // Do    { ApplyAttack, ApplyPush, UpdateHistory }
        public void InventoryPreset()
        {
            _CheckChain.AddMany(SetTargetsHandler, SetStatsHandler);
            _DoChain.AddMany(ApplyAttacksHandler, ApplyPushesHandler);            
        }

        public void NoInventoryPreset()
        {
            _CheckChain.AddMany(SetTargetsHandler, SetStatsHandler);
            _DoChain.AddMany(ApplyAttacksHandler, ApplyPushesHandler);            
        }

        public void AutoPreset(Entity entity)
        {
            if (entity.HasInventory())
                InventoryPreset();
            else
                NoInventoryPreset();
        }
    }
}