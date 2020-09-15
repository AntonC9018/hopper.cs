namespace Core.Items
{
    public class TinkerItem : IItem
    {
        private readonly int m_id;
        private readonly int m_slot;
        public int Slot => m_slot;
        public int Id => m_id;
        private ITinker tinker;

        public TinkerItem(ITinker tinker, int slot = 0)
        {
            this.tinker = tinker;
            m_slot = slot;
            m_id = IdMap.Items.Add(this);
        }

        public void BeDestroyed(Entity entity)
        {
            entity.Untink(tinker);
        }

        public void BeEquipped(Entity entity)
        {
            entity.TinkAndSave(tinker);
        }

        public void BeUnequipped(Entity entity)
        {
            entity.Untink(tinker);
            entity.World.CreateDroppedItem(this, entity.Pos);
        }
    }
}