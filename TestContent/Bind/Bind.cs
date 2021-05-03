using Hopper.Core;
using Hopper.Utils.Chains;
using Hopper.Core.Components.Basic;
using System;
using Hopper.Shared.Attributes;
using Hopper.Core.Targeting;
using Hopper.Core.Components;
using Hopper.Utils.Vector;
using System.Linq;
using Hopper.Core.Stat;
using Hopper.Utils;

namespace Hopper.TestContent.Bind
{
    // Added to the host when the guest binds itself
    public partial class BoundEntityModifier : IComponent
    {
        [Inject] public Entity guest;

        // When the time comes to attack, attack that entity instead
        [Export(Chain = "Attacking.Do", Priority = PriorityRank.High, Dynamic = true)]
        public void AttackBinder(Attacking.Context context)
        {
            context.SetSingleTarget(guest.GetTransform());
        }

        // When the host dies, free the guest
        [Export(Chain = "Damageable.Death", Dynamic = true)]
        public void FreeGuest()
        {
            guest.GetBinding().HostDiedCallback(guest);
        }

        // No disaplacements
        [Export(Chain = "Moving.Do", Priority = PriorityRank.High, Dynamic = true)]
        public bool StopDisplacement()
        {
            // continue if the guest is dead
            // return guest.IsDead();
            // This literally does not even matter, because if the guest did die,
            // this handler will not be here. I guess the only concern are other events,
            // but those cannot happen without leaving the current cell
            // This means we are free to just return false here.
            return false;
        }

        // TODO: apply attacks of type explosion to the guest instead
        public static HandlerGroupsWrapper DefaultHookable = new HandlerGroupsWrapper(
            AttackBinderHandlerWrapper,
            FreeGuestHandlerWrapper,
            StopDisplacementHandlerWrapper
        );
    }

    // Added to the entity that want to do binding.
    public partial class Binding : IComponent, IStandartActivateable
    {
        // The hookable that defines the exact effect applying 
        // this type of bind will have on the target entity.
        [Inject] public Layer targetedLayer;
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

            if (!Stat.Bind.Source.CheckResistance(target.entity, stat.power))
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