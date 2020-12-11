using System.Collections.Generic;

namespace Hopper.Core
{
    public class KindRegistry<T> : IKindRegistry<T> where T : IKind
    {
        // contains all items listed by mods in the order they were intialized.
        // we assume initialization takes place in the same sequence on both 
        // client and server, which is required for ids to match intended ones.
        protected Dictionary<string, List<T>> m_contentByMod = new Dictionary<string, List<T>>();
        // represents the local map. Once ids are generated at client, they are set in stone here
        public List<T> m_items = new List<T>();

        public IEnumerable<T> Items => m_items;

        public T Get(int id)
        {
            return m_items[id];
        }

        public int Add(T item, string modName = "Default")
        {
            if (!m_contentByMod.ContainsKey(modName))
            {
                m_contentByMod[modName] = new List<T>();
            }
            m_contentByMod[modName].Add(item);
            m_items.Add(item);
            return m_items.Count - 1;
        }
    }
}