namespace Core.Items
{
    // The problem is that the item has no control over the wat it's added in the inventory
    // thing is, it is added by it's ID. If we had e.g. a triple bomb item we would expect it
    // to be decomposed into three bomb items. The question is, where do I handle it: in the 
    // inventory or in the containers.
    // there's two ways to handle this
    public class PackedItem : Item
    {
        private IItem m_storedItem;
        private int m_count;

        public override ISlot Slot => m_storedItem.Slot;

        public PackedItem(IItem storedItem, int count) : base()
        {
            m_storedItem = storedItem;
            m_count = count;
        }

        public override void BeDestroyed(Entity entity)
        {
            for (int i = 0; i < m_count; i++)
            {
                m_storedItem.BeDestroyed(entity);
            }
        }

        public override void BeEquipped(Entity entity)
        {
            for (int i = 0; i < m_count; i++)
            {
                m_storedItem.BeEquipped(entity);
            }
        }

        public override DecomposedItem Decompose()
            => new DecomposedItem(m_storedItem, m_count);
    }
}