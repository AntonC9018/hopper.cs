using Core;
using Core.Items;

namespace Test
{
    public class CheckInventoryItem : TinkerItem
    {
        public CheckInventoryItem(ItemMetadata meta, ITinker tinker, ISlot slot) : base(meta, tinker, slot)
        {
            System.Console.WriteLine($"Tinker is null? : {tinker == null}");
        }

        private bool IsNotEquipped(Entity entity)
        {
            return entity.Inventory.IsEquipped(this) == false;
        }

        public override void BeDestroyed(Entity entity)
        {
            if (IsNotEquipped(entity))
            {
                base.BeDestroyed(entity);
            }
        }

        public override void BeEquipped(Entity entity)
        {
            if (IsNotEquipped(entity) && !m_tinker.IsTinked(entity))
            {
                base.BeEquipped(entity);
            }
        }
    }
}
