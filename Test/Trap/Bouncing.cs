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
        private bool m_hasBounced;
        private bool m_isEnterListenerApplied;
        private Layer m_targetedLayer = Layer.REAL;

        [DataMember] private bool m_hasEntityBeenOnTop;


        public override void Init(Entity entity, BehaviorConfig config)
        {
            m_hasBounced = false;
            m_entity = entity;

            // automatically update the whether an entity left on top of us
            m_entity.InitEvent += (() => m_entity.Cell.LeaveEvent += GetUnpushed);
            m_entity.DieEvent += (() => m_entity.Cell.LeaveEvent -= GetUnpushed);

            Tick.Chain.ChainPath(entity.Behaviors).AddHandler(Reset);
        }

        public bool Activate(Action action)
        {
            // if anybody has been standing on top since the previous loop, don't bounce
            // unless the entity gets off of us, which is managed by the leave handler
            if (m_hasEntityBeenOnTop)
            {
                m_isEnterListenerApplied = true;
                m_entity.Cell.EnterEvent += PushTarget;
                return true;
            }

            // otherwise, try to bounce
            var entity = m_entity.Cell.GetEntityFromLayer(m_targetedLayer);

            // if there's nobody to target, watch the cell, since
            // other traps may push a person onto us.
            if (entity == null)
            {
                m_isEnterListenerApplied = true;
                m_entity.Cell.EnterEvent += PushTarget;
            }

            // otherwise, push the thing which is on top of us.
            else
            {
                PushTarget(entity);
            }

            return true;
        }

        private void PushTarget(Entity oneBeingBounced)
        {
            if (ShouldPush(oneBeingBounced))
            {
                // if the entity actually gets pushed, this will be unset
                m_hasEntityBeenOnTop = true;

                var pushable = oneBeingBounced.Behaviors.TryGet<Pushable>();
                if (pushable == null) return;

                // bounce is considered applied even if the check doesn't go through
                m_hasBounced = true;

                pushable.Activate(
                    m_entity.Orientation,
                    m_entity.Stats.Get(Push.Path));
            }
        }

        private bool ShouldPush(Entity oneBeingBounced)
        {
            return m_hasBounced == false
                && m_hasEntityBeenOnTop == false // the previous entity has left
                && m_entity.IsDead == false
                && (oneBeingBounced.Layer & m_targetedLayer) != 0;
        }

        private void GetUnpushed(Entity leavingEntity)
        {
            // we don't push the entity in the next iteration 
            // if they end up on top of us in this iteration
            var ent = m_entity.Cell.GetEntityFromLayer(m_targetedLayer);
            m_hasEntityBeenOnTop = ent != null;

        }

        private void Reset(Tick.Event ev)
        {
            if (m_isEnterListenerApplied)
            {
                m_entity.Cell.EnterEvent -= PushTarget;
                m_isEnterListenerApplied = false;
            }
            m_hasBounced = false;
        }
    }
}