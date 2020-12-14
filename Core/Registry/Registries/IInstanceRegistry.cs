using System.Collections.Generic;
using Hopper.Utils;

namespace Hopper.Core
{
    public interface IInstanceSubRegistry
    {
    }

    public class InstanceSubRegistry<T, Meta> : IInstanceSubRegistry
    {
        protected Dictionary<int, T> m_map =
            new Dictionary<int, T>();

        protected Dictionary<int, Meta> m_meta =
            new Dictionary<int, Meta>();

        protected int m_currentId = 0;

        public void Reset() { }

        public int Add(T instance, Meta metadata)
        {
            m_currentId++;
            m_map[m_currentId] = instance;
            m_meta[m_currentId] = metadata;
            return m_currentId;
        }

        public T MapInstance(int id)
        {
            if (m_map.ContainsKey(id))
                return m_map[id];
            return default(T);
        }

        public Meta MapMetadata(int id)
        {
            if (m_meta.ContainsKey(id))
                return m_meta[id];
            return default(Meta);
        }

        public void Remove(int id)
        {
            Assert.That(m_map.Remove(id));
            Assert.That(m_meta.Remove(id));
        }
    }

    public class InstanceRegistry<T> : IInstanceSubRegistry
    {
        protected Dictionary<int, T> m_map =
            new Dictionary<int, T>();

        protected int m_currentId = 0;

        public void Reset() { }

        public int Add(T instance)
        {
            m_currentId++;
            m_map[m_currentId] = instance;
            return m_currentId;
        }

        public T MapInstance(int id)
        {
            if (m_map.ContainsKey(id))
                return m_map[id];
            return default(T);
        }

        public void Remove(int id)
        {
            m_map.Remove(id);
        }
    }
}