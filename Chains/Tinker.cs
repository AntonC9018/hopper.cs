using System.Collections.Generic;
using Chains;
using Core.Behaviors;
using Utils;
using Handle = Utils.MyLinkedList.MyListNode<Chains.IEvHandler>;

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
        protected IChainDef[] m_chainDefinition;
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();

        protected abstract TinkerData InstantiateData();

        public TinkerData CreateDataAndTink(Entity entity)
        {
            var data = InstantiateData();
            // we have to do this manually to not pass the length into the init function
            data.chainHandlesArray = new Handle[m_chainDefinition.Length][];
            data.Init(entity); // since the constructor can only be parameterless 

            for (int i = 0; i < m_chainDefinition.Length; i++)
            {
                var chainDef = m_chainDefinition[i];
                var handles = chainDef.AddHandlers(entity);
                data.chainHandlesArray[i] = handles;
            }

            return data;
        }

        public void Untink(TinkerData data, Entity entity)
        {
            for (int i = 0; i < data.chainHandlesArray.Length; i++)
            {
                var chainDef = m_chainDefinition[i];
                chainDef.RemoveHandlers(data.chainHandlesArray[i], entity);
            }
        }
    }

    public class Tinker<T> : ITinker where T : TinkerData, new()
    {
        public Tinker(IChainDef[] chainDefs)
        {
            m_chainDefinition = chainDefs;
        }
        protected override TinkerData InstantiateData() => new T();
        public T GetStore(Entity actor) => (T)actor.GetTinkerStore(this);
        public T GetStore(CommonEvent ev) => (T)ev.actor.GetTinkerStore(this);

        // beacuse I'm sick of boilerplate for simple stuff
        public static Tinker<T> SingleHandlered<Event>(
            IChainPaths<Event> path,
            System.Action<Event> handler,
            PriorityRanks priority = PriorityRanks.Default)
            where Event : EventBase
        {
            return new Tinker<T>(
                new IChainDef[]
                {
                    new ChainDef<Event>
                    {
                        path = path.ChainPath,
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