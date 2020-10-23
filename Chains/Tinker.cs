using Chains;
using Core.Behaviors;
using Newtonsoft.Json;

namespace Core
{
    public class TinkerData
    {
        [JsonIgnore] public Handle[][] chainHandlesArray;
        public int count = 1;
    }

    public interface ITinker : IHaveId
    {
        void Untink(Entity entity);
        void Tink(Entity entity);
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

        public void Tink(Entity entity)
        {
            Tink(entity, new T());
        }

        public void Tink(Entity entity, T tinkerData)
        {
            if (entity.Tinkers.IsTinked(this))
            {
                GetStore(entity).count++;
            }
            else
            {
                entity.Tinkers.Store(this, tinkerData);
                TinkHandlers(entity.Behaviors, tinkerData);
            }
        }

        private void TinkHandlers(BehaviorControl behaviors, TinkerData data)
        {
            data.chainHandlesArray = new Handle[m_chainDefinition.Length][];

            for (int i = 0; i < m_chainDefinition.Length; i++)
            {
                var chainDef = m_chainDefinition[i];
                var handles = chainDef.AddHandlers(behaviors);
                data.chainHandlesArray[i] = handles;
            }
        }

        private void UntinkHandlers(TinkerData data, IProvideBehavior behaviors)
        {
            for (int i = 0; i < data.chainHandlesArray.Length; i++)
            {
                var chainDef = m_chainDefinition[i];
                chainDef.RemoveHandlers(data.chainHandlesArray[i], behaviors);
            }
        }

        public void Untink(Entity entity)
        {
            var data = GetStore(entity);
            data.count--;

            if (data.count == 0)
            {
                entity.Tinkers.RemoveStore(this);
                UntinkHandlers(data, entity.Behaviors);
            }
        }

        public bool IsTinker(Entity entity) => entity.Tinkers.IsTinked(this);

        public T GetStore(Entity actor) => (T)actor.Tinkers.GetStore(this);
        public T GetStore(TinkerControl tinker) => (T)tinker.GetStore(this);
        public T GetStore(ActorEvent ev) => (T)ev.actor.Tinkers.GetStore(this);

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