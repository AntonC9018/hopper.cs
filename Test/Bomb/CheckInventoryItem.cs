using Core;
using Core.Items;

namespace Test
{
    public class CheckInventoryItem : TinkerItem
    {
        public CheckInventoryItem(ITinker tinker, int slot = 0) : base(tinker, slot)
        {
            System.Console.WriteLine($"Tinker is null? : {tinker == null}");
        }

        public override void BeDestroyed(Entity entity)
        {
            if (entity.Inventory.IsEquipped(this) == false)
                base.BeDestroyed(entity);
        }

        public override void BeEquipped(Entity entity)
        {
            if ((entity.Inventory.IsEquipped(this) || m_tinker.IsTinked(entity)) == false)
                base.BeEquipped(entity);
        }

        public override void BeUnequipped(Entity entity)
        {
            if (entity.Inventory.IsEquipped(this) == false)
                base.BeDestroyed(entity);

            entity.World.CreateDroppedItem(this, entity.Pos);
        }
    }
}