using Hopper.Core.Registries;
using Hopper.Core.Components;
using System.Collections.Generic;

namespace Hopper.Core
{
    // TODO: make this a struct!
    public sealed class Entity : IHaveId
    {
        public int Id => id;
        public int id;

        // Do it the easy way, with classes, for now.
        // In the future, maybe switch to value types and store them in central place, 
        // but I think that's a little too much for C#.
        public Dictionary<Identifier, IComponent> components;

        public void AddComponent<T>(Index<T> index, T component) where T : IComponent
        {
            components[index.componentId] = component;
        }

        public T GetComponent<T>(Index<T> index) where T : IComponent
        {
            return (T)components[index.componentId];
        }

        public T TryGetComponent<T>(Index<T> index) where T : IComponent
        {
            if (components.TryGetValue(index.componentId, out var component))
            {
                return (T)component;
            }
            return default(T);
        }

        public bool TryGetComponent<T>(Index<T> index, out T component) where T : IComponent
        {
            bool result = components.TryGetValue(index.componentId, out IComponent comp);
            component = (T)comp;
            return result;
        }

        public bool HasComponent<T>(Index<T> index) where T : IComponent
        {
            return components.ContainsKey(index.componentId);
        }

        public override bool Equals(object obj)
        {
            return id == (obj as Entity)?.id;
        }

        public override int GetHashCode() => id;
    }
}