using Hopper.Core;
using Hopper.Core.ActingNS;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Core.WorldNS;
using Hopper.Shared.Attributes;
using static Hopper.Core.ActingNS.Action;

namespace Hopper.TestContent.PinningNS
{
    public partial class PinnedEntityModifier : IComponent, IUndirectedActivateable
    {
        [Inject] public int amount;
        [Inject] public Entity pinner;

        public bool MaybeRemoveImmediately(Entity actor)
        {
            if (pinner.IsDead() || 
                actor.GetTransform().position != pinner.GetTransform().position)
            {
                RemoveFrom(actor);
                return true; 
            }
            return false;
        }

        public bool PreventActionAndDamagePinner(Entity actor)
        {
            if (MaybeRemoveImmediately(actor))
            {
                return false;
            }

            amount--;

            if (amount == 0)
            {
                RemoveFrom(actor);
            }

            // The action succeeds if the pinner has not already been dead
            // The action must succeed
            return true;
        }

        bool IUndirectedActivateable.Activate(Entity entity) => PreventActionAndDamagePinner(entity);

        // TODO: This regularly creates a bunch of trash temporaries on the heap.
        //       Save this in a static variable and initialize via [RequiringInit]
        //       (currently, they only work with fields)
        public static Identifier[] AffectedActions => 
            new Identifier[] { Moving.Index.Id, Attacking.Index.Id, Digging.Index.Id };

        [Export(Chain = "Acting.SubstituteAction", Dynamic = true)]
        public void ResetAction(Entity actor, ref CompiledAction currentAction)
        {
            if (!MaybeRemoveImmediately(actor)
                && currentAction.GetStoredAction().ActivatesEither(AffectedActions))
            {
                currentAction = currentAction.WithAction(
                    Compose(UAction, currentAction.GetStoredAction()));
            }
        }

        // If the entity is being pushed, suck in the push and damage the pinner
        [Export(Chain = "Pushable.Do", Dynamic = true)]
        public bool StopPush(Entity actor) => PreventActionAndDamagePinner(actor);

        public static void Preset(Entity entity)
        {
            ResetActionHandlerWrapper.TryHookTo(entity);
            StopPushHandlerWrapper.TryHookTo(entity);
        }

        public static void Unset(Entity entity)
        {
            ResetActionHandlerWrapper.TryUnhookFrom(entity);
            StopPushHandlerWrapper.TryUnhookFrom(entity);
        }

        public static void RemoveFrom(Entity entity)
        {
            Unset(entity);
            entity.RemoveComponent(Index);
        }
    }
}