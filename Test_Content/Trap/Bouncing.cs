using System.Runtime.Serialization;
using Hopper.Core;
using Hopper.Core.Behaviors;
using Hopper.Core.Behaviors.Basic;
using Hopper.Core.Stats.Basic;
using Hopper.Utils;
using Hopper.Utils.Vector;

namespace Hopper.Test_Content.Trap
{
    [DataContract]
    public class Bouncing : Behavior, IStandartActivateable
    {
        private bool m_hasBounced;
        private bool m_isEnterListenerApplied;
        private Layer m_targetedLayer = Layer.REAL;

        [DataMember] private bool m_hasEntityBeenOnTop;


        private void Init(object _)
        {
            m_hasBounced = false;
            // automatically update the whether an entity left on top of us
            m_entity.InitEvent += (() => m_entity.GetCell().LeaveEvent += GetUnpushed);
            m_entity.DieEvent += (() => m_entity.GetCell().LeaveEvent -= GetUnpushed);
        }

        public bool Activate(Action action)
        {
            if (m_entity.Orientation == IntVector2.Zero)
            {
                throw new Exception("The one pushing should have a direction to have any effect");
            }

            // if anybody has been standing on top since the previous loop, don't bounce
            // unless the entity gets off of us, which is managed by the leave handler
            if (m_hasEntityBeenOnTop)
            {
                StartListening();
                return true;
            }

            // otherwise, try to bounce
            var entity = m_entity.GetCell().GetUndirectedEntityFromLayer(m_targetedLayer);

            // if there's nobody to target, watch the cell, since
            // other traps may push a person onto us.
            if (entity == null)
            {
                StartListening();
            }

            // otherwise, push the thing which is on top of us.
            else
            {
                TryPushTarget(entity);
                m_hasBounced = false;
            }

            return true;
        }

        private void StartListening()
        {
            m_isEnterListenerApplied = true;
            m_entity.GetCell().EnterEvent += TryPushTarget;

            // only push in our phase
            m_entity.World.State.EndOfPhaseEvent += Reset;
        }

        private void TryPushTarget(Entity oneBeingBounced)
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
                    m_entity.Stats.GetLazy(Push.Path));
            }
        }

        private bool ShouldPush(Entity oneBeingBounced)
        {
            return m_hasBounced == false
                && m_hasEntityBeenOnTop == false // the previous entity has left
                && m_entity.IsDead == false
                && oneBeingBounced.IsOfLayer(m_targetedLayer)
                && oneBeingBounced.IsDirected == false;
        }

        private void GetUnpushed(Entity leavingEntity)
        {
            // we don't push the entity in the next iteration 
            // if they end up on top of us in this iteration
            var ent = m_entity.GetCell().GetUndirectedEntityFromLayer(m_targetedLayer);
            m_hasEntityBeenOnTop = ent != null;
        }

        private void Reset(Phase phase)
        {
            if (m_isEnterListenerApplied)
            {
                m_entity.GetCell().EnterEvent -= TryPushTarget;
                m_entity.World.State.EndOfPhaseEvent -= Reset;
                m_isEnterListenerApplied = false;
            }
            m_hasBounced = false;
        }
    }
}