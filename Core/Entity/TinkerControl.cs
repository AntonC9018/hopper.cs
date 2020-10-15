using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core
{
    [DataContract]
    public class TinkerControl
    {
        // tinker's storage. The elements of this map are processed exclusively by tinkers
        // and the handlers added by them.
        [DataMember]
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

        public void Store(ITinker tinker, TinkerData tinkerData)
        {
            if (IsTinked(tinker))
            {
                var store = m_tinkerStore[tinker.Id];
                store.count++;
            }
            else
            {
                m_tinkerStore[tinker.Id] = tinkerData;
            }
        }

        public void RemoveStore(ITinker tinker)
        {
            var store = m_tinkerStore[tinker.Id];
            store.count--;
            if (store.count == 0)
            {
                m_tinkerStore.Remove(tinker.Id);
            }
        }

        public TinkerData GetStore(ITinker tinker)
        {
            return m_tinkerStore.ContainsKey(tinker.Id) ? m_tinkerStore[tinker.Id] : null;
        }
    }
}