using Hopper.Core;
using Hopper.Core.Items;
using Hopper.Core.WorldNS;
using Hopper.Shared.Attributes;
using Hopper.Utils.Vector;

namespace Hopper.TestContent.Items
{
    [EntityType]
    public static class Gold
    {
        public static EntityFactory Factory;

        public static void AddComponents(Entity subject)
        {
            ItemBase.AddComponents(subject);

            // Item stuff
            Equippable.AddTo(subject, null);
            Countable.AddTo(subject);
        }

        public static void InitComponents(Entity subject)
        {
        }

        public static void Retouch(Entity subject)
        {
        }

        public static Entity Drop(IntVector2 position, int amount)
        {
            if (World.Global.Grid.GetCellAt(position).TryGetAnyFromLayer(Layer.ITEM, out var transform)
                && transform.entity.typeId == Factory.id)
            {
                var countable = transform.entity.GetCountable();
                countable.count += amount;
                return transform.entity;
            }
            var gold = World.Global.SpawnEntity(Factory, position, IntVector2.Zero);
            gold.GetCountable().count = amount;
            return gold;
        }

        [Slot] public static Slot Slot = new Slot(false);
    }
}