using System.Linq;
using Hopper.Core;
using Hopper.Core.ActingNS;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Core.WorldNS;
using Hopper.Shared.Attributes;
using Hopper.TestContent.Stat;
using Hopper.Utils.Chains;
using Hopper.Utils.Vector;
using static Hopper.Core.ActingNS.Action;

namespace Hopper.TestContent.SlidingNS
{
    public static partial class Sliding
    {
        [Chain("+Applied")]
        public static readonly Index<Chain<Entity>> Applied = new Index<Chain<Entity>>();
        
        [Chain("+Removed")]
        public static readonly Index<Chain<Entity>> Removed = new Index<Chain<Entity>>();

        public static readonly HandlerGroupsWrapper HandlerGroup = new HandlerGroupsWrapper(
            SlidingEntityModifier.AdjustDirectionOfSlidingAfterPushHandlerWrapper, 
            SlidingEntityModifier.SlideInsteadHandlerWrapper, 
            SlidingEntityModifier.MaybeStopSlidingHandlerWrapper);
    }

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
            return actor.TryBePushed(DefaultPush, direction);
        }

        // If the entity tries to move (on its own), slide instead
        [Export(Chain = "Acting.SubstituteAction", Dynamic = true)]
        public void SlideInstead(ref CompiledAction currentAction)
        {
            currentAction = Compose(Action, currentAction.GetStoredAction()).Compile(directionOfSliding);
        }

        // When displaced into something that is not slippery, stop sliding.
        // Also, stop sliding if slid into a wall.
        [Export(Chain = "Displaceable.After", Priority = PriorityRank.Lowest, Dynamic = true)]
        public static void MaybeStopSliding(Transform transform, IntVector2 direction)
        {
            // If none of the floor is slippery or the next thing is a block
            if (!transform.GetAllButSelfFromLayer(Layers.FLOOR).Any(t => t.entity.HasSlipperyComponent())
                || transform.HasBlockRelative(direction, Layers.BLOCK))
            {
                RemoveFrom(transform.entity);
            }
        }

        public static void Preset(Entity actor)
        {
            Sliding.HandlerGroup.TryHookTo(actor);
        }

        public static void Unset(Entity actor)
        {
            Sliding.HandlerGroup.TryUnhookFrom(actor);
        }

        public static void RemoveFrom(Entity actor)
        {
            Unset(actor);
            actor.RemoveComponent(Index);
            Sliding.RemovedPath.GetIfExists(actor)?.Pass(actor);
        }

        public static void TryApplyTo(Transform transform, IntVector2 directionOfSliding)
        {
            var actor = transform.entity;

            if (!actor.HasSlidingEntityModifier() 
                && directionOfSliding != IntVector2.Zero
                // No walls ahead
                && !transform.HasBlockRelative(directionOfSliding, Layers.BLOCK))
            {
                SlidingEntityModifier.AddTo(actor, directionOfSliding);
                Preset(actor);
                Sliding.AppliedPath.GetIfExists(actor)?.Pass(actor);
            }
        }
    }
}