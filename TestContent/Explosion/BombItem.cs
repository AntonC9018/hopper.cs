using Hopper.Core;
using Hopper.Core.Items;
using Hopper.Shared.Attributes;
using Hopper.Utils.Vector;
using static Hopper.Core.Action;

namespace Hopper.TestContent
{
    [EntityType]
    public static class BombItem
    {
        public static EntityFactory Factory;
        [Slot("Bomb")] public static Slot Slot = new Slot(false);

        public static SimpleAction PlaceAction = Simple(Adapt(PlantFunction));

        public static void Place(IntVector2 position, IntVector2 orientation)
        {
            World.Global.SpawnEntity(BombEntity.Factory, position, orientation);
        }

        public static void PlantFunction(Entity actor, IntVector2 direction)
        {
            if (actor.TryGetBomb(out var bomb))
            {
                var countable = bomb.GetCountable();
                if (countable.count > 0)
                {
                    countable.count--;
                    
                    var transform = actor.GetTransform();
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

        public static SimpleAction GetAction(Entity actor, Entity owner)
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
}