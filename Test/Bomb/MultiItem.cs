using Core;
using Core.Items;

namespace Test
{
    public class MultiItem : Item
    {
        private IItem m_storedItem;
        private int m_count;

        public override int Slot => m_storedItem.Slot;

        public MultiItem(IItem storedItem, int count) : base()
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
    }
}