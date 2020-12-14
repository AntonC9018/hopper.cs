using System.Collections.Generic;
using Hopper.Utils;

namespace Hopper.Core
{
    public class KindSubRegistry<T> : IKindRegistry<T>
    {
        public IEnumerable<T> Items
        {
            get
            {
                //TODO
                yield return m_items[1];
            }
        }

        private Dictionary<int, T> m_items;
        private IdGenerator m_idGenerator;

        public KindSubRegistry()
        {
            m_items = new Dictionary<int, T>();
            m_idGenerator = new IdGenerator();
        }

        public T Get(int id)
        {
            return m_items[id];
        }

        public int Add(T item, int offset)
        {
            int id = m_idGenerator.GetNextId() + offset;

            Assert.That(!m_items.ContainsKey(id),
                $"Id overlap detected for {typeof(T).Name}. Review your mod offset.");

            m_items[id] = item;
            return id;
        }

        public void ResetLocal()
        {
            m_idGenerator.Reset();
        }
    }
}