using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Shared.Attributes;
using Hopper.Core.Components;

namespace Hopper.TestContent.BindingNS
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
}