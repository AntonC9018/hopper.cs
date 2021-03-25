using Hopper.Core.Registries;
using System.Collections.Generic;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stats;

namespace Hopper.Core
{
    public class EntityFactory<T> : Kind<IFactory<Entity>>, IFactory<T>, IProvideBehaviorFactory
        where T : Entity, new()
    {
        public event System.Action<T> InitEvent;
        public event System.Action<PatchArea> PostPatchEvent;
        public DefaultStats DefaultStats;

        private Dictionary<System.Type, IBehaviorFactory<Behavior>> m_behaviorFactories;

        public EntityFactory()
        {
            m_behaviorFactories = new Dictionary<System.Type, IBehaviorFactory<Behavior>>();
            AddBehavior(Tick.Preset);
        }

        public T Instantiate()
        {
            T entity = new T();

            // Instantiate and save behaviors
            foreach (var kvp in m_behaviorFactories)
            {
                var type = kvp.Key;
                var setting = kvp.Value;
                var behavior = setting.Instantiate(entity);
                entity.Behaviors.Add(type, behavior);
            }

            if (DefaultStats != null)
            {
                entity.Stats = new StatManager(DefaultStats);
            }

            InitEvent?.Invoke(entity);
            return entity;
        }

        public void PostPatch(PatchArea patchArea)
        {
            PostPatchEvent?.Invoke(patchArea);
            PostPatchEvent = null;
        }

        public EntityFactory<T> AddBehavior<Beh>(IBehaviorFactory<Beh> factory)
            where Beh : Behavior, new()
        {
            m_behaviorFactories.Add(typeof(Beh), factory);
            return this;
        }

        public EntityFactory<T> Retouch(Retoucher retoucher)
        {
            retoucher.Retouch(this);
            return this;
        }

        public IBehaviorFactory<U> GetBehaviorFactory<U>() where U : Behavior, new()
        {
            return (IBehaviorFactory<U>)m_behaviorFactories[typeof(U)];
        }

        public bool HasBehaviorFactory<U>() where U : Behavior, new()
        {
            return m_behaviorFactories.ContainsKey(typeof(U));
        }

        public EntityFactory<T> ReconfigureBehavior<U, Config>(Config config)
            where U : Behavior, IInitable<Config>, new()
        {
            var factory = (ConfigurableBehaviorFactory<U, Config>)m_behaviorFactories[typeof(U)];
            factory.config = config;
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
