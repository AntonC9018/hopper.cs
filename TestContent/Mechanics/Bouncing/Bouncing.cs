using System.Collections.Generic;
using System.Linq;
using Hopper.Core;
using Hopper.Core.ActingNS;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Core.WorldNS;
using Hopper.Shared.Attributes;
using Hopper.Utils;
using Hopper.Utils.Vector;

namespace Hopper.TestContent.BouncingNS
{
    public partial class Bouncing : IComponent, IUndirectedActivateable
    {
        public const Layers _targetedLayer = Layers.REAL;
        public HashSet<RuntimeIdentifier> _bouncedEntities;
        public bool _isPressedDown;

        public void InitInWorld(Transform transform)
        {
            _bouncedEntities = new HashSet<RuntimeIdentifier>();

            // add the things
            var entity = transform.entity;
            transform.SubsribeToPermanentEnterEvent(ctx => Enter(entity, ctx));
            transform.SubsribeToPermanentLeaveEvent(ctx => Leave(entity, ctx));
        }

        [Alias("Bounce")]
        public bool Activate(Entity actor)
        {
            var transform = actor.GetTransform();

            Assert.That(transform.orientation != IntVector2.Zero, "The one pushing must have a direction to have any effect");

            // otherwise, try to bounce
            var targetTransform = transform.GetAllUndirectedButSelfFromLayer(_targetedLayer).FirstOrDefault();

            // otherwise, push the thing which is on top of us.
            if (targetTransform == null || !_bouncedEntities.Add(targetTransform.entity.id))
            {
                return true;
            }

            TryPushTarget(transform, targetTransform);

            return true;
        }

        public static bool Enter(Entity actor, CellMovementContext context)
        {
            if (!actor.TryGetBouncing(out var bouncing) || actor.IsDead())
            {
                return false; // Remove this listener
            }

            // TODO: maybe separate the handlers by order to automate this?
            if (actor.IsCurrentOrderFavorable() && context.transform.layer.HasEitherFlag(_targetedLayer))
            {
                // If the set already contained that element
                if (!bouncing._bouncedEntities.Add(context.actor.id))
                {
                    bouncing._isPressedDown = true;
                }

                // If the trap is not pressed down, try pushing the entity
                else if (!bouncing._isPressedDown)
                {
                    // This will be unset, if the entity gets pushed 
                    bouncing._isPressedDown = true;
                    bouncing.TryPushTarget(actor.GetTransform(), context.transform);
                }

                // otherwise, an entity has already been on top of us, so do nothing
            }

            return true; // Keep this listener
        }

        public static bool Leave(Entity actor, CellMovementContext context)
        {
            if (!actor.TryGetBouncing(out var bouncing) || actor.IsDead())
            {
                return false;
            }

            if (actor.IsCurrentOrderFavorable()
                && World.Global.Grid.HasNoUndirectedTransformAt(context.initialPosition, _targetedLayer))
            {
                bouncing._isPressedDown = false;
            }

            return true;
        }

        [Export(Chain = "Ticking.Do", Dynamic = true)]
        public void Reset(Entity actor)
        {
            var transform = actor.GetTransform();
            _bouncedEntities.Clear();
            
            if (transform.GetAllUndirectedButSelfFromLayer(_targetedLayer).Any())
            {
                _isPressedDown = true;
            }
        }

        private void TryPushTarget(Transform actor, Transform oneBeingBounced)
        {
            if (!oneBeingBounced.entity.TryGetPushable(out var pushable)) return;

            actor.entity.GetStats().GetLazy(Push.Index, out var push);
            pushable.Activate(oneBeingBounced.entity, push, actor.orientation);
        }

        public void DefaultPreset(Entity subject)
        {
            ResetHandlerWrapper.HookTo(subject);
        }
    }
}