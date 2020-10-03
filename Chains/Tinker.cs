using Chains;
using Core.Behaviors;
using Newtonsoft.Json;
using Handle = Utils.MyLinkedList.MyListNode<Chains.IEvHandler>;

namespace Core
{
    public class TinkerData
    {
        [JsonIgnore] public Handle[][] chainHandlesArray;
        public int count = 1;

        public virtual void Init(Entity entity) { }
    }

    public interface ITinker : IHaveId
    {
        TinkerData CreateDataAndTink(Entity entity);
        void Untink(TinkerData data, IProvideBehavior behaviors);
    }

    public class Tinker<T> : ITinker where T : TinkerData, new()
    {
        public int Id => m_id;
        protected readonly int m_id;
        protected IChainDef[] m_chainDefinition;

        public Tinker(IChainDef[] chainDefs)
        {
            m_chainDefinition = chainDefs;
            m_id = IdMap.Tinker.Add(this);
        }

        public TinkerData CreateDataAndTink(Entity entity)
        {
            var data = new T();
            // we have to do this manually to not pass the length into the init function
            // TODO: maybe remove the init function
            data.Init(entity); // since the constructor can only be parameterless
            Tink(entity.Behaviors, data);
            return data;
        }

        public void Tink(BehaviorControl behaviors, TinkerData data)
        {
            data.chainHandlesArray = new Handle[m_chainDefinition.Length][];
            
            for (int i = 0; i < m_chainDefinition.Length; i++)
            {
                var chainDef = m_chainDefinition[i];
                var handles = chainDef.AddHandlers(behaviors);
                data.chainHandlesArray[i] = handles;
            }
        }

        public void Untink(TinkerData data, IProvideBehavior behaviors)
        {
            for (int i = 0; i < data.chainHandlesArray.Length; i++)
            {
                var chainDef = m_chainDefinition[i];
                chainDef.RemoveHandlers(data.chainHandlesArray[i], behaviors);
            }
        }

        public T GetStore(Entity actor) => (T)actor.Tinkers.GetStore(this);
        public T GetStore(TinkerControl tinker) => (T)tinker.GetStore(this);
        public T GetStore(CommonEvent ev) => (T)ev.actor.Tinkers.GetStore(this);

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