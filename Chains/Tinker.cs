using System.Collections.Generic;
using Chains;
using Handle = MyLinkedList.MyListNode<Chains.IEvHandler>;

namespace Core
{
    public class TinkerData
    {
        public Handle[][] chainHandlesArray;
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
            data.chainHandlesArray = new Handle[m_chainDefinition.Length][];
            data.Init(entity); // since the constructor can only be parameterless 
            m_store[entity.id] = data;

            for (int i = 0; i < m_chainDefinition.Length; i++)
            {
                var chainDef = m_chainDefinition[i];
                var handles = chainDef.AddHandlersWithHandlesTo(entity);
                data.chainHandlesArray[i] = handles;
            }
        }
        public void Untink(Entity entity)
        {
            m_store[entity.id].count--;
            if (m_store[entity.id].count > 0) return;

            var data = m_store[entity.id];
            for (int i = 0; i < data.chainHandlesArray.Length; i++)
            {
                var chainDef = m_chainDefinition[i];
                chainDef.RemoveHandlersWithHandles(data.chainHandlesArray[i], entity);
            }
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

        // beacuse I'm sick of boilerplate for simple stuff
        public static Tinker<T> SingleHandlered<Event>(
            System.Func<IProvideBehavior, ICanAddHandlers<Event>> path,
            System.Action<Event> handler,
            PRIORITY_RANKS priority = PRIORITY_RANKS.DEFAULT)
            where Event : EventBase
        {
            return new Tinker<T>(
                new IChainDef[]
                {
                    new IChainDef<Event>
                    {
                        path = path,
                        handlers = new EvHandler<Event>[]
                        {
                            new EvHandler<Event>(handler, priority)
                        }
                    }
                }
            );
        }
    }
}