using Hopper.Core.Stat;
using Hopper.Core.Components;
using Hopper.Core.Stat.Basic;

namespace Hopper.Core
{
    public struct EntityModifierIndex<T> where T : IEntityModifier
    {
        public System.Func<Entity, T> InstantiateAndBind;
        public System.Action<T, Entity> Unbind;
        public StatusSource Source;
        public Index<T> ComponentIndex;

        public EntityModifierIndex(System.Func<Entity, T> InstantiateAndBind, System.Action<T, Entity> Unbind) 
            : this(InstantiateAndBind, Unbind, StatusSource.Resistance.Default)
        {
        }

        public EntityModifierIndex(System.Func<Entity, T> Instantiate, System.Action<T, Entity> Unbind, System.Func<StatusSource.Resistance> DefaultResistance) : this()
        {
            this.InstantiateAndBind = Instantiate;
            this.Source.Default = DefaultResistance;
        }

        public void Init()
        {
            ComponentIndex.Id = Registry.Global.NextComponentId();
            Source.Index.Id   = Registry.Global.RegisterStat(Source.Default());
        }

        public bool TryApplyTo(Entity entity, int power)
        {
            if (entity.TryGetStats(out var stats))
            {
                stats.GetLazy(Source.Index, out var resistance);
                if (resistance.amount >= power)
                {
                    ApplyTo(entity);
                    return true;
                }
            }
            return false;
        }

        public void ApplyTo(Entity entity)
        {
            if (!entity.HasComponent(ComponentIndex))
            {
                var component = InstantiateAndBind(entity);

                if (component != null)
                {
                    entity.AddComponent(ComponentIndex, component);
                }
            }
        }

        public void RemoveFrom(Entity entity)
        {
            var component = entity.GetComponent(ComponentIndex);
            Unbind(component, entity);
            entity.RemoveComponent(ComponentIndex);
        }

        public bool TryRemoveFrom(Entity entity)
        {
            if (entity.TryGetComponent(ComponentIndex, out var component))
            {
                Unbind(component, entity);
                entity.RemoveComponent(ComponentIndex);
                return true;
            }
            return false;
        }

        // public static implicit operator Index<T>(EntityModifierIndex<T> index) => index.ComponentIndex;
    }
}