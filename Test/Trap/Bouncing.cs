using System.Collections.Generic;
using System.Runtime.Serialization;
using Core;
using Core.Behaviors;
using Core.Stats.Basic;

namespace Test
{
    [DataContract]
    public class Bouncing : Behavior, IStandartActivateable
    {
        private HashSet<Entity> m_alreadyBouncedEntities;
        private System.Action<Entity> m_appliedHandlerEnter;
        private System.Action<Entity> m_appliedHandlerLeave;
        private Layer m_targetedLayer = Layer.REAL;

        [DataMember] private Entity m_entityOnTop; // TODO: use the right serializer

        private void PushTarget(Entity oneBeingBounced)
        {
            if (ShouldPush(oneBeingBounced))
            {
                oneBeingBounced.Behaviors.Get<Pushable>()
                    ?.Activate(
                        m_entity.Orientation,
                        m_entity.Stats.Get(Push.Path));
            }
            m_entityOnTop = oneBeingBounced;
        }

        private bool ShouldPush(Entity oneBeingBounced)
        {
            return m_entity.IsDead == false
                && !m_alreadyBouncedEntities.Contains(oneBeingBounced)
                && (oneBeingBounced.Layer & m_targetedLayer) != 0;
        }

        private void GetUnpushed(Entity leavingEntity)
        {
            if (leavingEntity == m_entityOnTop)
            {
                m_entityOnTop = m_entity.Cell.GetEntityFromLayer(m_targetedLayer);
            }
        }

        private void Reset(Tick.Event ev)
        {
            if (m_appliedHandlerEnter != null)
            {
                m_entity.Cell.EnterEvent -= m_appliedHandlerEnter;
                m_alreadyBouncedEntities.Clear();
                m_appliedHandlerEnter = null;

                m_entity.Cell.LeaveEvent -= m_appliedHandlerLeave;
                m_appliedHandlerLeave = null;
            }
            m_entityOnTop = m_entity.Cell.GetEntityFromLayer(m_targetedLayer);
        }

        public override void Init(Entity entity, BehaviorConfig config)
        {
            m_alreadyBouncedEntities = new HashSet<Entity>();
            m_entity = entity;
            Tick.Chain.ChainPath(entity.Behaviors).AddHandler(Reset);
        }

        public bool Activate(Action action)
        {
            m_appliedHandlerEnter = PushTarget;
            m_appliedHandlerLeave = GetUnpushed;
            m_entity.Cell.EnterEvent += m_appliedHandlerEnter;
            m_entity.Cell.LeaveEvent += m_appliedHandlerLeave;

            var entity = m_entity.Cell.GetEntityFromLayer(m_targetedLayer);
            if (entity != null && entity != m_entityOnTop)
            {
                PushTarget(entity);
            }

            return true;
        }
    }
}