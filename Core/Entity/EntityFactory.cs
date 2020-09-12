using System.Collections.Generic;
using Core.Behaviors;

namespace Core
{
    public class BehaviorSetting
    {
        public BehaviorFactory factory;
        public BehaviorConfig config;
    }

    public class EntityFactory<T> : IEntityFactory, IProvideBehaviorFactory
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

        public void AddBehavior<Beh>(BehaviorConfig conf = null)
            where Beh : Behavior, new()
        {
            var factory = new BehaviorFactory<Beh>();
            var setting = new BehaviorSetting { factory = factory, config = conf };
            m_behaviorSettings.Add(typeof(Beh), setting);
        }

        public void RetouchAndSave(Retoucher retoucher)
        {
            m_retouchers.Add(retoucher.id, retoucher);
            retoucher.Retouch(this);
        }

        public bool IsRetouched(Retoucher retoucher)
        {
            return m_retouchers.ContainsKey(retoucher.id);
        }

        public Entity Instantiate()
        {
            Entity entity = new T();
            // Instantiate and save behaviors
            foreach (var (t, setting) in m_behaviorSettings)
            {
                var behavior = setting.factory.Instantiate(entity, setting.config);
                entity.AddBehavior(t, behavior);
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
