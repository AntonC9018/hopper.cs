using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Shared.Attributes;
using Hopper.Core.Components;
using Hopper.Core.WorldNS;

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
        [Export(Chain = "+Entity.Death", Dynamic = true)]
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
    }

    // This is a workaround
    // I cannot control the order of initialization of static variables.
    // I cannot leave that thing in the static class and initialize it at declaration, 
    // because it references other static declarations in the other partial part of the class.
    // I cannot define a static constructor, because it is already defined by the code generator. 
    // I need the static contructor there because I cannot intialize Paths correctly, 
    // since, again, the order of intialization is undefined, but they depend on user-defined
    // static fields.
    // So the workaround is to define static classes for static data that doesn't have to do with code generation.
    // I see no other solution.
    // This works, because accessing the static fields in another class forces the static contructor to fire,
    // which means the fields will be initialized correctly.
    public static class BoundEntityModifierDefault
    {
        // TODO: apply attacks of type explosion to the guest instead
        public static HandlerGroupsWrapper Hookable = new HandlerGroupsWrapper(
            BoundEntityModifier.AttackBinderHandlerWrapper,
            BoundEntityModifier.FreeGuestHandlerWrapper,
            BoundEntityModifier.StopDisplacementHandlerWrapper
        );
    }
}