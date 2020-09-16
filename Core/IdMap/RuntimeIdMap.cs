using System.Collections.Generic;

namespace Core
{
    public class InstanceAndFactory
    {
        public IHaveId instance;
        public IHaveId factory;

        public InstanceAndFactory(IHaveId instance, IHaveId factory)
        {
            this.instance = instance;
            this.factory = factory;
        }
    }

    public class RuntimeIdMap
    {
        protected Dictionary<int, InstanceAndFactory> m_map = new Dictionary<int, InstanceAndFactory>();
        protected int m_currentId;

        // public async void RestoreState()
        // {

        //     m_map.Clear();
        // var currentId = await idProxy.GetCurrentId();
        // if (IdMap.IsInRuntimePhase)
        // {
        //     m_currentId = currentId;
        // }
        // }

        public IHaveId Map(int id)
        {
            if (m_map.ContainsKey(id))
                return m_map[id].instance;
            return null;
        }

        public void Remove(int id)
        {
            m_map.Remove(id);
        }

        public int Add(IHaveId instance, IHaveId factory)
        {
            m_currentId++;
            m_map[m_currentId] = new InstanceAndFactory(instance, factory);
            return m_currentId;
        }

        // public int GenerateId()
        // {
        //     m_currentId++;
        //     return m_currentId;
        // }
    }

    public class RuntimeIdMap<T, FactoryT> : RuntimeIdMap
        where T : IHaveId
    {
        public int Add(T instance, FactoryT factory)
        {
            return base.Add(instance, (IHaveId)factory);
        }
    }
}