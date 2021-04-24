using System.Collections.Generic;
using Hopper.Core;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Core.Stat.Basic;
using Hopper.Core.Targeting;
using Hopper.Shared.Attributes;
using Hopper.Utils;
using Hopper.Utils.Vector;

namespace Hopper.TestContent.Projectiles
{
    public class ProjectileBehavior : IComponent, IStandartActivateable
    {
        [Inject] public Layer targetedLayer;
        public Cell _cellBeingWatched;
        public bool _isWatching;

        /// <summary>
        /// Attacks the entity looking in the specified direction. Otherwise, 
        /// moves in the specified direction, after which tries to attack whatever is
        /// at the same cell as the projectile at that point. Would not attack if provided direction of zero.
        /// Would start watching the cell's <c>EnterEvent</c> and attack entities that enter the cell 
        /// until the end of loop. Attacking is only done on entities at the <c>targetLayer</c>. 
        /// </summary>
        /// <returns>Always returns true.</returns>
        /// <remarks>
        /// In the future, should add a `Do` chain that should be traversed instead of attacking entities directly.
        /// </remarks>
        public bool Activate(Entity actor, IntVector2 direction)
        {
            // Assert.AreNotEqual(IntVector2.Zero, direction, "Must be moving somewhere");
            var transform = actor.GetTransform();
            var stats = actor.GetStats();
            stats.GetLazy(Attack.Index, out var attack);

            // If the projectile is not floating at one spot
            if (direction != IntVector2.Zero)
            {
                Assert.That(!actor.GetDisplaceable().blockLayer.HasFlag(Layer.WALL),
                    "We should be able to move INTO walls");

                // First, attack entities, that are looking at us, standing at the same cell as us.
                // Ignore any directed entities.

                foreach (var targetTransform in transform.GetAllUndirectedButSelfFromLayer(targetedLayer))
                {
                    if (targetTransform.orientation != -direction) continue;

                    if (targetTransform.entity.TryBeAttacked(actor, attack, transform.orientation))
                    {
                        actor.Die();
                        return true;
                    }
                }

                stats.GetLazy(Move.Index, out var move);
                actor.TryDisplace(direction, move);
            }

            if (!actor.IsDead())
            {
                // Attack the entity (entities) from the same cell, but not oneself.
                foreach (var targetTransform in transform.GetAllButSelfFromLayer(targetedLayer))
                {
                    if (targetTransform.entity.TryBeAttacked(actor, attack, transform.orientation))
                    {
                        actor.Die();
                        return true;
                    }
                }

                // Attack anything that enters the spot.
                {
                    _cellBeingWatched = actor.GetCell();
                    _cellBeingWatched.EnterEvent += HitEntered;
                }
            }

            return true;
        }

        private void Hit(Entity actor)
        {
            Attacking.TryApplyAttack(
                attacked    : actor,
                direction   : actor.GetTransform().,
                attack      : actor.GetStats().GetLazy(Attack.Index),
                attacker    : actor
            );
            actor.Die(); // For now, just die. Add a do chain later.
        }

        private void TryStopWatching()
        {
            if (_isWatching)
            {
                _cellBeingWatched.EnterEvent -= HitEntered;
                _cellBeingWatched = null;
            }
        }

        private void HitEntered(Entity enteredEntity)
        {
            if (actor.IsDead)
            {
                TryStopWatching();
            }
            else if (enteredEntity.IsOfLayer(targetedLayer))
            {
                Hit(enteredEntity);
            }
        }
    }
}