using Hopper.Core.Components;
using System;
using System.Collections.Generic;

namespace Hopper.Core
{
    // TODO: make this a struct!
    public sealed class Entity
    {
        public Identifier typeId;
        public RuntimeIdentifier id;


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
            components.Remove(index.Id);
        }

        public bool TryRemoveComponent<T>(Index<T> index) where T : IComponent
        {
            return components.Remove(index.Id);
        }
        
        public void AddComponent<T>(Index<T> index, T component) where T : IComponent
        {
            components.Add(index.Id, component);
        }

        public T GetComponent<T>(Index<T> index) where T : IComponent
        {
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