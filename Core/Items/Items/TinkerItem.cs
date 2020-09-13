namespace Core.Items
{
    public class TinkerItem : Item
    {
        ITinker tinker;

        public TinkerItem(ITinker tinker, int slot = 0)
        {
            this.tinker = tinker;
            this.slot = slot;
        }

        public override void BeDestroyed(Entity entity)
        {
            entity.Untink(tinker);
        }

        public override void BeEquipped(Entity entity)
        {
            entity.TinkAndSave(tinker);
        }

        public override void BeUnequipped(Entity entity)
        {
            entity.Untink(tinker);
            entity.World.CreateDroppedItem(this, entity.Pos);
        }
    }
}