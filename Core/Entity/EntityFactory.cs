using System.Collections.Generic;
using Hopper.Core.Behaviors;
using Hopper.Core.Utils;

namespace Hopper.Core
{
    public class BehaviorSetting
    {
        public IBehaviorFactory factory;
        public object config;
    }

    public class EntityFactory<T> : IFactory<T>, IProvideBehaviorFactory
        where T : Entity, new()
    {

        public int Id => m_id;
        private int m_id;
        public event System.Action<T> SetupEvent;

        public EntityFactory()
        {
            m_id = Registry.Default.EntityFactory.Add(this);
            AddBehavior<Tick>();
        }

        private Dictionary<System.Type, BehaviorSetting> m_behaviorSettings =
            new Dictionary<System.Type, BehaviorSetting>();

        private Dictionary<int, Retoucher> m_retouchers =
            new Dictionary<int, Retoucher>();

        public EntityFactory<T> AddBehavior<Beh>(object conf = null)
            where Beh : Behavior, new()
        {
            var factory = new BehaviorFactory<Beh>();
            var setting = new BehaviorSetting { factory = factory, config = conf };
            m_behaviorSettings.Add(typeof(Beh), setting);
            return this;
        }

        public EntityFactory<T> Retouch(Retoucher retoucher)
        {
            m_retouchers.Add(retoucher.Id, retoucher);
            retoucher.Retouch(this);
            return this;
        }

        public bool IsRetouched(Retoucher retoucher)
        {
            return m_retouchers.ContainsKey(retoucher.Id);
        }

        public T Instantiate()
        {
            var entity = InstantiateLogic();
            int id = Registry.Default.Entity.Add(entity, new FactoryLink { factoryId = m_id });
            entity._SetId(id);
            return entity;
        }

        public T ReInstantiate(int id)
        {
            var entity = InstantiateLogic();
            entity._SetId(id);
            return entity;
        }

        private T InstantiateLogic()
        {
            T entity = new T();

            // Instantiate and save behaviors
            foreach (var kvp in m_behaviorSettings)
            {
                var type = kvp.Key;
                var setting = kvp.Value;
                var behavior = setting.factory.Instantiate(entity, setting.config);
                entity.Behaviors.Add(type, behavior);
            }
            SetupEvent?.Invoke(entity);
            return entity;
        }

        public BehaviorFactory<U> GetBehaviorFactory<U>() where U : Behavior, new()
        {
            return (BehaviorFactory<U>)m_behaviorSettings[typeof(U)].factory;
        }

        public bool HasBehaviorFactory<U>() where U : Behavior, new()
        {
            return m_behaviorSettings.ContainsKey(typeof(U));
        }

        public EntityFactory<T> ReconfigureBehavior<U>(object config) where U : Behavior, new()
        {
            m_behaviorSettings[typeof(U)].config = config;
            return this;
        }

        public EntityFactory<T> AddSetupListener(System.Action<T> listener)
        {
            SetupEvent += listener;
            return this;
        }

        public EntityFactory<T> AddInitListener(System.Action<T> listener)
        {
            // call the listener once the entity gets assigned 
            // a position in the world for the first time
            SetupEvent += (e => e.InitEvent += () => listener(e));
            return this;
        }

        public EntityFactory<T> AddDieListener(System.Action<T> listener)
        {
            // call the listener once the entity dies
            SetupEvent += (e => e.DieEvent += () => listener(e));
            return this;
        }

        public EntityFactory<T> SetDefaultStats(Hopper.Core.Stats.DefaultStats stats)
        {
            return AddSetupListener(e => e.Stats.DefaultStats = stats.StatManager);
        }
    }
}
