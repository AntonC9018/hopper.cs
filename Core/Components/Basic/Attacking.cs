using System.Collections.Generic;
using System.Linq;
using Hopper.Core.Targeting;
using Hopper.Core.Stat;
using Hopper.Utils.Vector;
using Hopper.Shared.Attributes;
using Hopper.Core.ActingNS;
using Hopper.Core.WorldNS;
using Hopper.Core.Items;
using Hopper.Utils.Chains;

namespace Hopper.Core.Components.Basic
{
    public partial class Attacking : IBehavior, IStandartActivateable, IPredictable
    {
        public class Context : StandartContext
        {
            public AttackTargetingContext targetingContext;

            public Context(Entity actor, IntVector2 direction)
            {
                this.actor = actor;
                this.direction = direction;
            }

            public void SetSingleTarget(Transform target)
            {
                var transform = actor.GetTransform();

                targetingContext = new AttackTargetingContext(
                    new List<AttackTargetContext>(1) { new AttackTargetContext(transform) },
                    pattern          : null, 
                    targetedLayer    : target.layer, 
                    blockLayer       : 0);
            }

            public void AttackTargets(Attack attack)
            {
                foreach (var target in targetingContext.targetContexts)
                {
                    if (!target.transform.entity.IsDead())
                    {
                        target.transform.entity.TryBeAttacked(actor, attack, direction);
                    }
                }
            }

            public void PushTargets(Push push)
            {
                foreach (var target in targetingContext.targetContexts)
                {
                    if (!target.transform.entity.IsDead())
                    {
                        target.transform.entity.TryBePushed(push, direction);
                    }
                }
            }
        }

        /// <summary>
        /// The prediction system needs the underlying pattern to show the endangered cells.
        /// We have to go one step down from the chain abstraction, in order to get to this pattern.
        /// Since this target provider may be either selected statically, or drawn dynamically 
        /// from the inventory, or some other source, I say we abstract it away a little bit
        /// by using a function that would get us the target provider.
        /// TODO:
        /// Now we may instead inject the provider itself, in which case it will have to be directly
        /// synced when e.g. the weapon is changed. This is also a viable option, so this about it.
        /// </summary>
        [Inject] public readonly System.Func<Entity, BufferedAttackTargetProvider> GetDefaultAttackTargetProvider;
        [Inject] public Layers targetedLayer;
        [Inject] public Faction targetedFaction;

        // [Chain("FilterTargets")] private Chain<Context> _FilterTargetsChain;

        [Chain("Check")]      private Chain<Context> _CheckChain;
        [Chain("SetTargets")] private Chain<Context> _SetTargetsChain;
        [Chain("After")]      private Chain<Context> _AfterChain;

        [Alias("Attack")]
        public bool Activate(Entity actor, IntVector2 direction)
        {
            var context = new Context(actor, direction);

            // If the targets have not been set by any external handlers, set it manually here
            if (!_SetTargetsChain.PassUntil(context, ctx => ctx.targetingContext != null))
            {
                var targetProvider = GetDefaultAttackTargetProvider(actor);

                // Let's just say the action fails if that function returned nothing.
                if (targetProvider is null) return false;

                var targetingContext = targetProvider.GetTargets(actor, targetedLayer, direction);

                // This filters the targets by the leaving the ones of the correct faction.
                targetingContext.LeaveTargetsOfFaction(targetedFaction);

                context.targetingContext = targetingContext;
            }

            // The next chain checks if the action should be done.
            // As a rule, this would include a function that would check if e.g. the attack is not empty.
            if (!_CheckChain.PassWithPropagationChecking(context))
            {
                return false;
            }

            var stats = actor.GetStats();

            // Now we can do the action normally.
            // Apply all attacks
            context.AttackTargets(stats.GetLazy(Attack.Index));
            context.PushTargets(stats.GetLazy(Push.Index));

            _AfterChain.Pass(context);

            return true;
        }

        /// <summary>
        /// Returns the target provider from the current weapon of the given entity.
        /// If they hold no weapon, null will be returned.
        /// </summary>
        public static BufferedAttackTargetProvider GetTargetProviderFromInventory(Entity actor)
        {
            if (actor.TryGetInventory(out var inventory)
                && inventory.TryGetWeapon(out var weapon)
                && weapon.TryGetBufferedAttackTargetProvider(out var provider))
            {
                return provider;
            }
            return null;
        }

        /// <summary>
        /// Returns the positions that would be attacked if the behavior's activate were to be called. 
        /// TODO: use the info on layers and the faction to return better estimations
        /// </summary>
        public IEnumerable<IntVector2> Predict(Entity actor, IntVector2 direction, PredictionTargetInfo info)
        {
            // If the entity would not be targeted, skip
            if (info.faction.HasNeitherFlag(targetedFaction) || info.layer.HasNeitherFlag(targetedLayer)) 
            {
                yield break;
            }

            // TODO: maybe generalize this to use the SetTargets chain.
            var provider = GetDefaultAttackTargetProvider(actor);
            if (provider is null) yield break;
            
            foreach (var ctx in provider._pattern.MakeContexts(actor.GetTransform().position, direction))
            {
                // This has more info, can use.
                // TODO: The info contains
                yield return ctx.position;
            }
        }

        public void SkipEmptyAttackPreset()
        {
            _CheckChain.Add(Hopper.Core.Retouchers.Skip.SkipEmptyAttackHandler);
        }
    }
}