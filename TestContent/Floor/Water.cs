using Hopper.Core;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Shared.Attributes;

namespace Hopper.TestContent.Floor
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
                return true;
            }

            amount--;

            if (amount == 0)
            {
                RemoveFrom(actor);
            }

            // The action fails if the pinner has not already been dead
            return false;
        }

        bool IUndirectedActivateable.Activate(Entity entity) => PreventActionAndDamagePinner(entity);

        // TODO: This regularly creates a bunch of trash temporaries on the heap.
        //       Save this in a static variable and initialize via [RequiringInit]
        //       (currently, they only work with fields)
        public static Identifier[] AffectedActions => 
            new Identifier[] { Moving.Index.Id, Attacking.Index.Id, Digging.Index.Id };

        [Export(Chain = "Acting.Check", Dynamic = true)]
        public void ResetAction(Entity actor, ref CompiledAction action)
        {
            if (!MaybeRemoveImmediately(actor)
                && action.GetStoredAction().ActivatesEither(AffectedActions))
            {
                action = action.WithAction(
                    UAction.Then(action.GetStoredAction())
                );
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

    public partial class PinningComponent : IComponent
    {
        public void InitInWorld(Transform transform)
        {
            var entity = transform.entity;
            transform.SubsribeToPermanentEnterEvent(ctx => Enter(entity, ctx));
        }

        public bool Enter(Entity actor, CellMovementContext ctx)
        {
            if (!actor.TryGetPinningComponent(out var pinning) || actor.IsDead())
            {
                return false; // Remove this listener
            }

            if (ctx.actor.HasPinnedEntityModifier() || ctx.HasMoved())
            {
                return true;
            }

            actor.GetStats().GetLazy(Stat.PinStat.Index, out var pinStat);

            if (ctx.actor.CanNotResist(Stat.PinStat.Source, pinStat.power))
            {
                PinnedEntityModifier.AddTo(ctx.actor, pinStat.amount, actor);
                PinnedEntityModifier.Preset(ctx.actor);
            }
            return true;
        }
    }

    [EntityType]
    public static class Water
    {
        public static EntityFactory Factory;

        public static void AddComponents(Entity subject)
        {
            Stats.AddTo(subject, Registry.Global._defaultStats);
            Transform.AddTo(subject, Layer.FLOOR);
            FactionComponent.AddTo(subject, Faction.Environment);
            Damageable.AddTo(subject, new Health(1));
            PinningComponent.AddTo(subject);
        }

        public static void InitComponents(Entity subject)
        {
            subject.GetDamageable().DefaultPreset();
        }

        public static void Retouch(EntityFactory factory)
        {
            Stats.AddInitTo(factory);
            PinningComponent.AddInitTo(factory);
        }
    }
}