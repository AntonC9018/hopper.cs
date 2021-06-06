using Hopper.Core;
using Hopper.Shared.Attributes;
using Hopper.Core.Components;
using Hopper.Utils.Vector;
using Hopper.Core.Stat;
using Hopper.Core.Components.Basic;
using Hopper.Utils;
using Hopper.Core.ActingNS;
using Hopper.Core.WorldNS;

namespace Hopper.TestContent.BindingNS
{
    // Added to the entity that want to do binding.
    public partial class Binding : IComponent, IStandartActivateable
    {
        // The hookable that defines the exact effect applying 
        // this type of bind will have on the target entity.
        [Inject] public Layers targetedLayer;
        [Inject] public IHookable hookable;
        public Transform host;

        public bool Activate(Entity actor, IntVector2 direction)
        {
            if (host != null)
            {
                return false;
            }
            if (direction == IntVector2.Zero)
            {
                return false;
            }
            var transform = actor.GetTransform();
            var targets = transform.GetAllUndirectedButSelfFromLayerRelative(targetedLayer, direction);
            
            foreach (var t in targets)
            {
                if (TryApplyTo(transform, t))
                {
                    return true;
                }
            }
            return false;
        }

        public bool TryApplyTo(Transform actor, Transform target)
        {
            Assert.That(host == null);
            
            if (target.entity.HasBoundEntityModifier())
            {
                return false;
            }

            actor.entity.GetStats().GetLazy(Stat.Bind.Index, out var stat);

            if (target.entity.CanNotResist(Stat.Bind.Source, stat.power))
            {
                ApplyTo(actor, target);
            }

            return true;
        }

        public void ApplyTo(Transform actor, Transform target)
        {
            BoundEntityModifier.AddTo(target.entity, actor.entity);

            // Now the exact effect is defined by the injects, so we can just hook it.
            hookable.TryHookTo(target.entity);

            // We're not done though. If the binding entity dies, the hookable needs to be removed.
            // The host also needs to be restored in the grid.
            GuestDiedCallbackHandlerWrapper.TryHookTo(actor.entity);

            // TODO: store these data for the viewmodel somehow
            host = target;
            actor.RemoveFromGrid();
            actor.position = target.position;
        }

        [Export(Chain = "Damageable.Death", Dynamic = true)]
        public void GuestDiedCallback()
        {
            // The spider is the one who's not in the grid
            // host.ResetInGrid();
            hookable.TryUnhookFrom(host.entity);
            host.entity.TryRemoveComponent(BoundEntityModifier.Index);
        }

        public void HostDiedCallback(Entity actor)
        {
            actor.GetTransform().ResetInGrid();
            host = null;
        }

        [Alias("IsCurrentlyBinding")]
        public bool IsApplied()
        {
            return host != null;
        }

        
        public void DefaultPreset()
        {
            hookable = BoundEntityModifier.DefaultHookable;
        }
    }
}