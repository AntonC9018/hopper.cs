namespace Core.Items
{
    public class TinkerItem : IItem
    {
        private readonly int m_id;
        private readonly int m_slot;
        public int Slot => m_slot;
        public int Id => m_id;
        protected ITinker m_tinker;

        public TinkerItem(ITinker tinker, int slot = 0)
        {
            this.m_tinker = tinker;
            m_slot = slot;
            m_id = IdMap.Items.Add(this);
        }

        public virtual void BeDestroyed(Entity entity)
        {
            m_tinker.Untink(entity);
        }

        public virtual void BeEquipped(Entity entity)
        {
            m_tinker.Tink(entity);
        }

        public virtual void BeUnequipped(Entity entity)
        {
            m_tinker.Untink(entity);
            entity.World.CreateDroppedItem(this, entity.Pos);
        }
    }
}