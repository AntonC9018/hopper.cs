using Hopper.Core;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Core.Items;
using Hopper.Shared.Attributes;
using Hopper.TestContent.SimpleMobs;
using Hopper.Utils.Vector;

namespace Hopper.TestContent
{
    public partial class WillExplodeEntityModifier : IComponent
    {
        [Inject] public int countDown;

        [Export(Chain = "Ticking.Do", Dynamic = true)]
        public void CountDown(Entity actor)
        {
            if (--countDown <= 0)
            {
                actor.TryDie();
            }
        }

        [Export(Chain = "Damageable.Death", Dynamic = true)]
        public void Explode(Entity actor)
        {
            Explosion.ExplodeBy(actor);
            Remove(actor);
        }

        public void Preset(Entity entity)
        {
            CountDownHandlerWrapper.HookTo(entity);
            ExplodeHandlerWrapper.HookTo(entity);
        }

        public void Remove(Entity actor)
        {
            actor.RemoveComponent(Index);
            CountDownHandlerWrapper.UnhookFrom(actor);
            ExplodeHandlerWrapper.UnhookFrom(actor);
        }
    }

    [EntityType]
    public static class BombItem
    {
        public static EntityFactory Factory;
        [Slot("Bomb")] public static Slot Slot = new Slot(false);

        public static DirectedAction PlaceAction = Action.CreateSimple(PlantFunction);

        public static void Place(IntVector2 position, IntVector2 orientation)
        {
            World.Global.SpawnEntity(BombEntity.Factory, position, orientation);
        }

        public static void PlantFunction(Acting acting, IntVector2 direction)
        {
            if (acting.actor.TryGetBomb(out var bomb))
            {
                var countable = bomb.GetCountable();
                if (countable.count > 0)
                {
                    countable.count--;
                    
                    var transform = acting.actor.GetTransform();
                    if (transform.HasBlockRelative(direction))
                    {
                        Place(transform.position, direction);
                    }
                    else
                    {
                        Place(transform.position + direction, direction);
                    }
                }
            }
        }

        public static Action GetAction(Entity actor, Entity owner)
        {
            if (actor.TryGetBomb(out var bomb))
            {
                var countable = bomb.GetCountable();
                if (countable.count > 0)
                {
                    return PlaceAction;
                }
            }
            return null;
        }

        public static void AddComponents(Entity subject)
        {
            ItemBase.AddComponents(subject);

            // Item stuff
            Equippable.AddTo(subject, null);
            SlotComponent.AddTo(subject, Slot.Id);
            ItemActivation.AddTo(subject, GetAction);
        }

        public static void InitComponents(Entity subject)
        {
            ItemBase.InitComponents(subject);
        }

        public static void Retouch(Entity subject)
        {
            ItemBase.Retouch(subject);
        }
    }


    [EntityType]
    public static class BombEntity
    {
        public static EntityFactory Factory;

        public static void AddComponents(Entity subject)
        {
            SimplePassiveRealBase.AddComponents(subject);
            WillExplodeEntityModifier.AddTo(subject, 3);
        }

        public static void InitComponents(Entity subject)
        {
            SimplePassiveRealBase.InitComponents(subject);
            subject.GetWillExplodeEntityModifier().Preset(subject);
        }

        public static void Retouch(Entity subject){}
    }
}