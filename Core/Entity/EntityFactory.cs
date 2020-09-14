using System.Collections.Generic;
using Core.Behaviors;

namespace Core
{
    public class BehaviorSetting
    {
        public BehaviorFactory factory;
        public BehaviorConfig config;
    }

    public class EntityFactory<T> : IInstantiateEntities, IProvideBehaviorFactory
        where T : Entity, new()
    {
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();
        public event System.Action<Entity> InitEvent;

        public EntityFactory()
        {
            AddBehavior<Tick>();
        }

        private Dictionary<System.Type, BehaviorSetting> m_behaviorSettings =
            new Dictionary<System.Type, BehaviorSetting>();

        private Dictionary<int, Retoucher> m_retouchers =
            new Dictionary<int, Retoucher>();

        public EntityFactory<T> AddBehavior<Beh>(BehaviorConfig conf = null)
            where Beh : Behavior, new()
        {
            var factory = new BehaviorFactory<Beh>();
            var setting = new BehaviorSetting { factory = factory, config = conf };
            m_behaviorSettings.Add(typeof(Beh), setting);
            return this;
        }

        public EntityFactory<T> RetouchAndSave(Retoucher retoucher)
        {
            m_retouchers.Add(retoucher.id, retoucher);
            retoucher.Retouch(this);
            return this;
        }

        public bool IsRetouched(Retoucher retoucher)
        {
            return m_retouchers.ContainsKey(retoucher.id);
        }

        public Entity Instantiate()
        {
            Entity entity = new T();
            // Instantiate and save behaviors
            foreach (var kvp in m_behaviorSettings)
            {
                var type = kvp.Key;
                var setting = kvp.Value;
                var behavior = setting.factory.Instantiate(entity, setting.config);
                entity.AddBehavior(type, behavior);
            }
            InitEvent?.Invoke(entity);
            return entity;
        }

        public BehaviorFactory<U> GetBehaviorFactory<U>() where U : Behavior, new()
        {
            return (BehaviorFactory<U>)m_behaviorSettings[typeof(U)].factory;
        }
    }
}
