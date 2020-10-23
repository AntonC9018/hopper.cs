using Core;
using Core.Items;

namespace Test
{
    public class MultiItem : IItem
    {
        private IItem m_storedItem;
        private int m_id;
        private int m_count;

        public int Slot => m_storedItem.Slot;
        public int Id => m_id;

        public MultiItem(IItem storedItem, int count)
        {
            m_storedItem = storedItem;
            m_count = count;
            m_id = IdMap.Items.Add(this);
        }

        public void BeDestroyed(Entity entity)
        {
            for (int i = 0; i < m_count; i++)
            {
                m_storedItem.BeDestroyed(entity);
            }
        }

        public void BeEquipped(Entity entity)
        {
            for (int i = 0; i < m_count; i++)
            {
                m_storedItem.BeEquipped(entity);
            }
        }

        public void BeUnequipped(Entity entity)
        {
            for (int i = 0; i < m_count; i++)
            {
                m_storedItem.BeUnequipped(entity);
            }
        }
    }
}