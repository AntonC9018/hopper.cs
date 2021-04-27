using Hopper.Core.Stat;
using Hopper.Core.Components;

namespace Hopper.Core
{
    public static class StatusSourceExtensions
    {
        public static bool CheckResistance(this StatusSource source, Entity entity, int power)
        {
            if (entity.TryGetStats(out var stats))
            {
                stats.GetLazy(source.Index, out var resistance);
                if (resistance.amount >= power)
                {
                    return true;
                }
            }
            return false;
        }
    }

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
            this.Unbind         = Unbind;
            this.Source.Default = DefaultResistance;
            this.InstantiateAndBind = Instantiate;
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