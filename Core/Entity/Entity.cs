using Hopper.Core.Components;
using Hopper.Utils;
using System.Collections.Generic;
using Hopper.Shared.Attributes;
using Hopper.Utils.Chains;
using Hopper.Core.WorldNS;
using Hopper.Core.Components.Basic;

namespace Hopper.Core
{
    [Flags] public enum EntityFlags
    {
        IsDead    = 1,
        /// <summary>
        /// Not currently used, added for consistency sake.
        /// </summary>
        IsInWorld = 1 << 1,
    }

    // TODO: make this a struct!
    [InstanceExport]
    public sealed partial class Entity
    {
        public Identifier typeId;
        public RuntimeIdentifier id;
        public EntityFlags flags;


        // Do it the easy way, with classes, for now.
        // In the future, maybe switch to value types and store them in central place, 
        // but I think that's a little too much for C#.
        public Dictionary<Identifier, IComponent> components;

        public bool TryAddComponent<T>(Index<T> index, T component) where T : IComponent
        {
            if (!components.ContainsKey(index.Id))
            {
                components[index.Id] = component;
                return true;
            }
            return false;
        }
        
        public void RemoveComponent<T>(Index<T> index) where T : IComponent
        {
            Assert.That(components.ContainsKey(index.Id), $"Cannot remove a non-existent component {index}.");
            components.Remove(index.Id);
        }

        public bool TryRemoveComponent<T>(Index<T> index) where T : IComponent
        {
            return components.Remove(index.Id);
        }
        
        public void AddComponent<T>(Index<T> index, T component) where T : IComponent
        {
            Assert.That(!components.ContainsKey(index.Id), $"Cannot add {index} twice.");
            components.Add(index.Id, component);
        }

        public T GetComponent<T>(Index<T> index) where T : IComponent
        {
            Assert.That(components.ContainsKey(index.Id), $"{index} not found among entities' components.");
            return (T)components[index.Id];
        }

        public IComponent GetSomeComponent(Identifier componentId)
        {
            return components[componentId];
        }

        public T TryGetComponent<T>(Index<T> index) where T : IComponent
        {
            if (components.TryGetValue(index.Id, out var component))
            {
                return (T)component;
            }
            return default(T);
        }

        public bool TryGetComponent<T>(Index<T> index, out T component) where T : IComponent
        {
            bool result = components.TryGetValue(index.Id, out IComponent comp);
            component = (T)comp;
            return result;
        }

        public bool HasComponent<T>(Index<T> index) where T : IComponent
        {
            return components.ContainsKey(index.Id);
        }

        public bool HasSomeComponent(Identifier id)
        {
            return components.ContainsKey(id);
        }

        public bool IsDead() => flags.HasFlag(EntityFlags.IsDead);

        /// <summary>
        /// Exports a death chain to MoreChains.
        /// The reason it is not in the entity itself is because it is probably unnecessary
        /// most of the time and we shouldn't clutter the entity.
        /// </summary>
        [Chain("+Death")] public static readonly Index<Chain<Entity>> DeathIndex = new Index<Chain<Entity>>();

        // TODO: This is a workaround for now.
        // I'm pretty sure the codegen won't work for exporting a chain with the context type
        // of `Entity` without this.
        public Entity actor => this;

        public void TryDie()
        {
            if (!IsDead()) Die();
        }

        public void Die()
        {
            Assert.False(IsDead());
            flags |= EntityFlags.IsDead;
            DeathPath.GetIfExists(this)?.Pass(this);
            // TODO: Shouldn't this be a handler on the transform?
            actor.GetTransform().TryRemoveFromGridWithoutEvent();
            // TODO: update indices on the registry? or should this be done via a handler in transform?
        }

        public override bool Equals(object obj)
        {
            return id == (obj as Entity)?.id;
        }

        public override int GetHashCode() => id.GetHashCode();
        
        public static bool operator==(Entity a, Entity b)
        {
            return a.id == b.id;
        }

        public static bool operator!=(Entity a, Entity b)
        {
            return a.id != b.id;
        }
    }
}