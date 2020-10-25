namespace Core.Items
{
    public class TinkerItem : Item
    {
        private readonly int m_slot;
        public override int Slot => m_slot;
        protected ITinker m_tinker;

        public TinkerItem(ITinker tinker, int slot = 0) : base()
        {
            this.m_tinker = tinker;
            m_slot = slot;
        }

        public override void BeDestroyed(Entity entity)
        {
            m_tinker.Untink(entity);
        }

        public override void BeEquipped(Entity entity)
        {
            m_tinker.Tink(entity);
        }
    }
}