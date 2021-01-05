using System.Collections.Generic;
using Hopper.Core;
using Hopper.Core.Behaviors;
using Hopper.Core.Behaviors.Basic;
using Hopper.Core.Stats.Basic;
using Hopper.Core.Targeting;
using Hopper.Utils;
using Hopper.Utils.Vector;

namespace Hopper.Test_Content.Projectiles
{
    public class ProjectileBehavior : Behavior, IStandartActivateable, IInitable<Layer>
    {
        public Layer targetedLayer;
        private Cell m_cellBeingWatched;

        public bool IsWatching => m_cellBeingWatched != null;

        public void Init(Layer layer)
        {
            targetedLayer = layer;
            Tick.Chain.ChainPath(m_entity.Behaviors)
                // .AddHandler(e => e.actor.Behaviors.Get<ProjectileBehavior>().TryStopWatching());
                .AddHandler(e => TryStopWatching());
        }

        /// <summary>
        /// Attacks the entity looking in the direction specified by action. Otherwise, 
        /// moves in the direction specified by action, after which tries to attack whatever is
        /// at the same cell as the projectile at that point. Would not attack if provided direction of zero.
        /// Would start watching the cell's <c>EnterEvent</c> and attack entities that enter the cell 
        /// until the end of loop. Attacking is only done on entities at the <c>targetLayer</c>, given at config. 
        /// </summary>
        /// <returns>Always returns true.</returns>
        /// <remarks>
        /// In the future, should add a `Do` chain that should be traversed instead of attacking entities directly.
        /// </remarks>
        public bool Activate(Action action)
        {
            // Assert.AreNotEqual(IntVector2.Zero, action.direction, "Must be moving somewhere");

            // If the projectile is not floating at one spot
            if (action.direction != IntVector2.Zero)
            {
                Assert.That(!m_entity.Behaviors.Get<Displaceable>().blockLayer.IsOfLayer(Layer.WALL),
                    "We should be able to move INTO walls");

                // First, attack entities, that are looking at us, standing at the same cell as us.
                // Ignore any directed entities.
                var target = m_entity.GetCell().m_entities.Find(
                    e => e.IsOfLayer(targetedLayer)
                        && !e.IsDirected
                        && e.Orientation == -action.direction
                        && e != m_entity
                );

                if (target != null)
                {
                    // Attack and die.
                    Hit(target);
                }
                if (!m_entity.IsDead)
                {
                    // Move in the direction specified by action.
                    var move = m_entity.Stats.GetLazy(Move.Path);
                    m_entity.Behaviors.Get<Displaceable>().Activate(action.direction, move);
                }
            }

            if (!m_entity.IsDead)
            {
                // Attack the entity (entities) from the same cell, but not oneself.
                foreach (var entity in m_entity.GetCell().GetAllFromLayer(m_entity.Orientation, targetedLayer))
                {
                    if (entity != m_entity)
                    {
                        Hit(entity);
                    }
                    if (m_entity.IsDead)
                    {
                        return true;
                    }
                }

                // Attack anything that enters the spot.
                {
                    m_cellBeingWatched = m_entity.GetCell();
                    m_cellBeingWatched.EnterEvent += HitEntered;
                }
            }

            return true;
        }

        private void Hit(Entity entity)
        {
            Attacking.TryApplyAttack(
                attacked: entity,
                direction: entity.Orientation,
                attack: m_entity.Stats.GetLazy(Attack.Path),
                attacker: m_entity
            );
            m_entity.Die(); // For now, just die. Add a do chain later.
        }

        private void TryStopWatching()
        {
            if (IsWatching)
            {
                m_cellBeingWatched.EnterEvent -= HitEntered;
                m_cellBeingWatched = null;
            }
        }

        private void HitEntered(Entity enteredEntity)
        {
            if (m_entity.IsDead)
            {
                TryStopWatching();
            }
            else if (enteredEntity.IsOfLayer(targetedLayer))
            {
                Hit(enteredEntity);
            }
        }

        public static ConfigurableBehaviorFactory<ProjectileBehavior, Layer> Preset(Layer targetedLayer) =>
            new ConfigurableBehaviorFactory<ProjectileBehavior, Layer>(null, targetedLayer);
    }
}