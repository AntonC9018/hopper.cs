using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Hopper.Core
{
    [DataContract]
    public class TinkerControl
    {
        // tinker's storage. The elements of this map are processed exclusively by tinkers
        // and the handlers added by them.
        [DataMember]
        private readonly Dictionary<int, TinkerData> m_tinkerStore =
            new Dictionary<int, TinkerData>();

        public bool IsTinked(ITinker tinker)
        {
            return m_tinkerStore.ContainsKey(tinker.Id);
        }

        public void Store(ITinker tinker, TinkerData tinkerData)
        {
            m_tinkerStore[tinker.Id] = tinkerData;
        }

        public void RemoveStore(ITinker tinker)
        {
            m_tinkerStore.Remove(tinker.Id);
        }

        public TinkerData GetStore(ITinker tinker)
        {
            return m_tinkerStore[tinker.Id];
        }
    }
}