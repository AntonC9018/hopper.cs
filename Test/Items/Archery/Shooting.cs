using System.Collections.Generic;
using Core;
using Core.Behaviors;
using Core.History;
using Core.Stats.Basic;
using Core.Utils.Vector;

namespace Test
{
    public abstract class Shooting
    {
        public static readonly UpdateCode UpdateCode = new UpdateCode("shooting");

        private Layer m_targetedLayer;
        private Layer m_skipLayer;
        protected bool m_stopOnFailedAttack;

        protected Shooting(Layer targetedLayer, Layer skipLayer, bool stopOnFailedAttack)
        {
            m_targetedLayer = targetedLayer;
            m_skipLayer = skipLayer;
            m_stopOnFailedAttack = stopOnFailedAttack;
        }

        public void Shoot(Entity entity, Action action)
        {
            ShootingPrelude(entity, action);
            ShootSpecific(entity, action);
        }

        protected void ShootingPrelude(Entity entity, Action action)
        {
            entity.Reorient(action.direction);
            entity.History.Add(entity, UpdateCode);
        }

        protected abstract void ShootSpecific(Entity entity, Action action);

        protected IEnumerable<Entity> GetShootTargets(Entity entity, Action action)
        {
            IntVector2 currentOffsetVec = action.direction;

            while (true)
            {
                var cell = entity.GetCellRelative(currentOffsetVec);

                // off the world or a block is in the way
                if (cell == null || cell.GetEntityFromLayer(m_skipLayer) != null)
                {
                    break;
                }

                var target = cell.GetEntityFromLayer(m_targetedLayer);

                yield return target;

                currentOffsetVec += action.direction;
            }
        }

        protected bool ShootAt(Entity attacker, Entity attacked, IntVector2 direction, Attack attack)
        {
            return attacked.Behaviors.Get<Attackable>()
                .Activate(direction, new Attackable.Params(attack, attacker));
        }

        protected void PushBack(Entity attacked, IntVector2 direction, Push push)
        {
            attacked.Behaviors.Get<Pushable>().Activate(direction, push);
        }
    }
}