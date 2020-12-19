using System.Collections.Generic;
using Hopper.Utils;

namespace Hopper.Core.Registries
{
    public class KindRegistry<T> : IKindRegistry<T>
    {
        public IEnumerable<T> Items
        {
            get
            {
                foreach (var it in m_items.Values)
                {
                    yield return it;
                }
            }
        }

        private Dictionary<int, T> m_items;
        private IdGenerator m_idGenerator;

        public KindRegistry()
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