using System.Linq;
using Hopper.Core;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Shared.Attributes;
using Hopper.TestContent.Stat;
using Hopper.Utils.Vector;

namespace Hopper.TestContent.Floor
{

    public partial class SlidingEntityModifier : IComponent, IStandartActivateable
    {
        [Inject] public IntVector2 directionOfSliding; 


        // When the entity is pushed, this direction should also be adjusted.
        [Export(Chain = "Pushable.Do", Priority = PriorityRank.Highest, Dynamic = true)]
        public void AdjustDirectionOfSlidingAfterPush(IntVector2 direction)
        {
            directionOfSliding = direction;
        }

        // A slide instead action that always fails.
        public static readonly Push DefaultPush = new Push
        (
            distance : 1,
            pierce   : 1,
            power    : 1,
            source   : Slide.PushSource.Index
        );

        public bool Activate(Entity actor, IntVector2 direction)
        {
            return !actor.TryBePushed(DefaultPush, direction);
        }

        // If the entity tries to move (on its own), slide instead
        [Export(Chain = "Acting.Check", Dynamic = true)]
        public void SlideInstead(ref CompiledAction action)
        {
            action = Action.Then(action._storedAction).Compile(directionOfSliding);
        }

        // When displaced into something that is not slippery, stop sliding.
        // Also, stop sliding if slid into a wall.
        [Export(Chain = "Displaceable.Do", Priority = PriorityRank.Low, Dynamic = true)]
        public static void MaybeStopSliding(Displaceable.Context ctx)
        {
            var transform = ctx.actor.GetTransform();

            // If none of the floor is slippery or the next thing is a block
            if (!transform.GetAllButSelfFromLayer(Layer.FLOOR).Any(t => t.entity.HasSlipperyComponent())
                || transform.HasBlockRelative(ctx.direction, ExtendedLayer.BLOCK))
            {
                RemoveFrom(ctx.actor);
            }
        }


        public static HandlerGroupsWrapper group = new HandlerGroupsWrapper(
            AdjustDirectionOfSlidingAfterPushHandlerWrapper, SlideInsteadHandlerWrapper, MaybeStopSlidingHandlerWrapper);

        public static void Preset(Entity actor)
        {
            group.TryHookTo(actor);
        }

        public static void Unset(Entity actor)
        {
            group.TryUnhookFrom(actor);
        }

        public static void RemoveFrom(Entity actor)
        {
            Unset(actor);
            actor.RemoveComponent(Index);
        }

        public static void TryApplyTo(Entity actor, IntVector2 directionOfSliding)
        {
            if (!actor.HasSlidingEntityModifier() && directionOfSliding != IntVector2.Zero)
            {
                SlidingEntityModifier.AddTo(actor, directionOfSliding);
                Preset(actor);
            }
        }
    }
}