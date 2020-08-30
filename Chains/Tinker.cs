using System.Collections.Generic;
using Core.Behaviors;
using Handle = MyLinkedList.MyListNode<Chains.IEvHandler>;

namespace Core
{
    public class ChainHandles
    {
        public string name;
        public Handle[] handles;
    }
    public class TinkerData
    {
        public ChainHandles[] chainHandlesArray;
        public int count = 1;
        public virtual void Init(Entity entity) { }
    }

    public abstract class ITinker
    {
        protected Dictionary<int, TinkerData> m_store;
        protected IChainDef[] m_chainDefinition;
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();

        public void RemoveStore(int entityId)
        {
            m_store.Remove(entityId);
        }
        public bool IsApplied(int entityId)
        {
            return m_store.ContainsKey(entityId);
        }
        protected abstract TinkerData InstantiateData();
        public void Tink(Entity entity)
        {
            if (m_store.ContainsKey(entity.id))
            {
                m_store[entity.id].count++;
                return;
            }

            var data = InstantiateData();
            // we have to do this manually to not pass the length into the init function
            data.chainHandlesArray = new ChainHandles[m_chainDefinition.Length];
            data.Init(entity); // since the constructor can only be parameterless 
            m_store[entity.id] = data;

            for (int i = 0; i < m_chainDefinition.Length; i++)
            {
                var chainDef = m_chainDefinition[i];
                var handles = new Handle[chainDef.handlers.Length];
                data.chainHandlesArray[i] = new ChainHandles
                {
                    name = chainDef.name,
                    handles = handles
                };
                var chain = entity.m_chains[chainDef.name];
                for (int j = 0; j < chainDef.handlers.Length; j++)
                {
                    var handler = chainDef.handlers[j];
                    handles[j] = chain.AddHandler(handler);
                }
            }
        }
        public void Untink(Entity entity)
        {
            m_store[entity.id].count--;
            if (m_store[entity.id].count > 0) return;

            var data = m_store[entity.id];
            foreach (var chainHandles in data.chainHandlesArray)
            {
                var chain = entity.m_chains[chainHandles.name];
                foreach (var handle in chainHandles.handles)
                {
                    chain.RemoveHandler(handle);
                }
            }
            m_store.Remove(entity.id);
        }
    }

    public class Tinker<T> : ITinker where T : TinkerData, new()
    {
        public Tinker(IChainDef[] chainDefs)
        {
            m_store = new Dictionary<int, TinkerData>();
            m_chainDefinition = chainDefs;
        }
        protected override TinkerData InstantiateData() => new T();
        public T GetStore(int entityId) => (T)m_store[entityId];
        public T GetStoreByEvent(CommonEvent ev) => (T)m_store[ev.actor.id];
    }
}