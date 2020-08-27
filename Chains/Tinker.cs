using System.Collections.Generic;
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
    }

    // TODO: refactor into a factory. probably
    // TODO: refactor into an abstract class or an interface
    public class Tinker
    {
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();
        Dictionary<int, TinkerData> m_store = new Dictionary<int, TinkerData>();
        // TODO: because this should be static
        // another possibility is to define static members separately for each Tinker class
        // and then set them in the constructor
        // the difference between this and the Behaviors is that there we
        // do not know in advance what we are instanciating
        // Also we keep track of the amount of inherits rather than the amount of instances
        // (for behaviors, that is)
        public IChainDef[] m_chainDefinition;

        // void AddStore(int entityId, TinkerData data)
        // {
        //     m_store[entityId] = data;
        // }

        public void RemoveStore(int entityId)
        {
            m_store.Remove(entityId);
        }

        // void GetStore(int entity)

        public bool IsApplied(int entityId)
        {
            return m_store.ContainsKey(entityId);
        }

        protected virtual TinkerData MakeData()
        {
            return new TinkerData
            {
                chainHandlesArray = new ChainHandles[m_chainDefinition.Length]
            };
        }

        public virtual void Tink(Entity entity)
        {

            if (m_store.ContainsKey(entity.id))
            {
                throw new System.Exception("You can't apply a tinker twice");
            }

            var data = MakeData();
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
}