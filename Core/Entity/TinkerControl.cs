using System.Collections.Generic;

namespace Core
{
    public class TinkerControl
    {
        // tinker's storage. The elements of this map are processed exclusively by tinkers
        // and the handlers added by them.
        private readonly Dictionary<int, TinkerData> m_tinkerStore =
            new Dictionary<int, TinkerData>();

        private Entity m_entity;

        public TinkerControl(Entity entity)
        {
            m_entity = entity;
        }

        public bool IsTinked(ITinker tinker)
        {
            return m_tinkerStore.ContainsKey(tinker.Id);
        }

        public void TinkAndSave(ITinker tinker)
        {
            if (IsTinked(tinker))
            {
                var store = m_tinkerStore[tinker.Id];
                store.count++;
            }
            else
            {
                m_tinkerStore[tinker.Id] = tinker.CreateDataAndTink(m_entity);
            }
        }

        public void Untink(ITinker tinker)
        {
            var store = m_tinkerStore[tinker.Id];
            store.count--;
            if (store.count == 0)
            {
                tinker.Untink(store, m_entity.Behaviors);
                m_tinkerStore.Remove(tinker.Id);
            }
        }

        public void TryUntink(ITinker tinker)
        {
            if (IsTinked(tinker))
                Untink(tinker);
        }

        public TinkerData GetStore(ITinker tinker)
        {
            return m_tinkerStore[tinker.Id];
        }
    }
}