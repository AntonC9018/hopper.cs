using System.Linq;
using Hopper.Core;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Shared.Attributes;
using Hopper.Utils;
using Hopper.Utils.Vector;

namespace Hopper.TestContent.Floor
{
    // TODO: This is incomplete. Needs some thought
    public partial class Bouncing : IComponent
    {
        public const Layer _targetedLayer = Layer.REAL;
        private bool _hasEntityBeenOnTop;
        private bool _hasBounced;

        [Alias("Bounce")]
        public bool Activate(Entity actor)
        {
            var transform = actor.GetTransform();

            Assert.That(transform.orientation != IntVector2.Zero, "The one pushing must have a direction to have any effect");

            // if anybody has been standing on top since the previous loop, don't bounce
            // unless the entity gets off of us, which is managed by the leave handler
            // TODO: this currenlty does not work, since the triggers are refreshed at the end of the same round 
            // This needs a bit more thought
            if (_hasEntityBeenOnTop)
            {
                StartListening(transform);
                return true;
            }

            // otherwise, try to bounce
            var entity = transform.GetAllUndirectedButSelfFromLayer(_targetedLayer).FirstOrDefault();

            // if there's nobody to target, watch the cell, since
            // other traps may push a person onto us.
            if (entity == null)
            {
                StartListening(transform);
            }

            // otherwise, push the thing which is on top of us.
            else
            {
                TryPushTarget(transform, entity);
            }

            return true;
        }

        private void StartListening(Transform actorTransform)
        {
            actorTransform.SubsribeToEnterEvent(ctx => TryPushTarget(actorTransform, ctx.transform));
            // TODO: same for leave, but that one must stay at least until the next round
        }

        private void TryPushTarget(Transform actor, Transform oneBeingBounced)
        {
            if (ShouldPush(actor, oneBeingBounced))
            {
                // if the entity actually gets pushed, this will be unset
                _hasEntityBeenOnTop = true;

                if (!oneBeingBounced.entity.TryGetPushable(out var pushable)) return;

                // bounce is considered applied even if the check doesn't go through
                _hasBounced = true;

                actor.entity.GetStats().GetLazy(Push.Index, out var push);

                pushable.Activate(oneBeingBounced.entity, push, actor.orientation);
            }
        }

        private bool ShouldPush(Transform actor, Transform oneBeingBounced)
        {
            return !_hasBounced
                && !_hasEntityBeenOnTop // the previous entity has left
                && !actor.entity.IsDead()
                && oneBeingBounced.layer.HasFlag(_targetedLayer)
                && !oneBeingBounced.entity.IsDirected();
        }
    }
}