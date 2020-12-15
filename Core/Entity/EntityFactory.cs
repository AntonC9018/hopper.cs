using Hopper.Core.Registry;
using System.Collections.Generic;
using Hopper.Core.Behaviors;
using Hopper.Core.Behaviors.Basic;
using Hopper.Core.Stats;

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
        public event System.Action<T> InitEvent;
        public event System.Action<PatchArea> PostPatchEvent;
        public DefaultStats DefaultStats;

        private Dictionary<System.Type, BehaviorSetting> m_behaviorSettings;
        private Dictionary<int, Retoucher> m_retouchers;

        public EntityFactory()
        {
            m_behaviorSettings = new Dictionary<System.Type, BehaviorSetting>();
            m_retouchers = new Dictionary<int, Retoucher>();
            AddBehavior<Tick>();
        }

        public T Instantiate()
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

            if (DefaultStats != null)
            {
                entity.Stats = new StatManager(DefaultStats);
            }

            InitEvent?.Invoke(entity);
            return entity;
        }

        public void RegisterSelf(ModRegistry subRegistry)
        {
            m_id = subRegistry.Add<IFactory<Entity>>(this);
        }

        public void PostPatch(PatchArea patchArea)
        {
            PostPatchEvent?.Invoke(patchArea);
            PostPatchEvent = null;
        }

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
            InitEvent += listener;
            return this;
        }

        public EntityFactory<T> AddInitListener(System.Action<T> listener)
        {
            // call the listener once the entity gets assigned 
            // a position in the world for the first time
            InitEvent += (e => e.InitEvent += () => listener(e));
            return this;
        }

        public EntityFactory<T> AddDieListener(System.Action<T> listener)
        {
            // call the listener once the entity dies
            InitEvent += (e => e.DieEvent += () => listener(e));
            return this;
        }

        public EntityFactory<T> SetDefaultStats(System.Func<PatchArea, DefaultStats> callback)
        {
            PostPatchEvent += (patchArea) => DefaultStats = callback(patchArea);
            return this;
        }
    }
}
