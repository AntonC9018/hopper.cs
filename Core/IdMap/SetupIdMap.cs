using System.Collections.Generic;
using Utils;

namespace Core
{
    public class SetupIdMap<T> : ISetupIdMap where T : IHaveId
    {
        // contains all items listed by mods in the order they were intialized.
        // we assume initialization takes place in the same sequence on both 
        // client and server, which is required for ids to match intended ones.
        protected Dictionary<string, List<T>> m_modMap = new Dictionary<string, List<T>>();
        // represents the local map. Once ids are generated at client, they are set in stone here
        protected List<T> m_clientMap = new List<T>();
        // provides a mapping of Server ids -> Client ids and backwards
        // this is used only for processing data coming from server and serialization (and backwards)
        protected Map<int, int> m_serverToClientMap = new Map<int, int>();

        public IEnumerable<T> ActiveItems
        {
            get
            {
                foreach (int clientId in m_serverToClientMap.Reverse.Keys)
                    yield return m_clientMap[clientId];
            }
        }

        public T Map(int id)
        {
            return m_clientMap[id];
        }

        public int Add(T item, string modName = "Default")
        {
            if (!m_modMap.ContainsKey(modName))
            {
                m_modMap[modName] = new List<T>();
            }
            m_modMap[modName].Add(item);
            m_clientMap.Add(item);
            return m_clientMap.Count - 1;
        }

        public void SetServerMap(List<MapInstruction> instructions)
        {
            m_serverToClientMap.Clear();
            for (int id = 0; id < instructions.Count; id++)
            {
                var instruction = instructions[id];
                var elementWithId = m_modMap[instruction.modName][instruction.listIndex];
                m_serverToClientMap.Add(id, elementWithId.Id);
            }
        }

        public List<MapInstruction> PackModMap()
        {
            var result = new List<MapInstruction>();
            foreach (var kvp in m_modMap)
            {
                for (int i = 0; i < kvp.Value.Count; i++)
                    result.Add(new MapInstruction(kvp.Key, i));
            }
            return result;
        }
    }
}