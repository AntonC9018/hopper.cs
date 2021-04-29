using System.Collections.Generic;
using Hopper.Core.Components;
using Hopper.Utils;

namespace Hopper.Core
{
    public sealed class EntityFactory
    {
        public Identifier id;
        public Entity subject;
        public System.Action<Transform> InitInWorldFunc;

        public EntityFactory()
        {
            subject.components = new Dictionary<Identifier, IComponent>();
        }

        public EntityFactory AddComponent<T>(Index<T> index, T component) where T : IComponent
        {
            subject.AddComponent(index, component);
            return this;
        }

        public T GetComponent<T>(Index<T> index) where T : IComponent
        {
            return subject.GetComponent(index);
        }

        public Entity Instantiate()
        {
            Entity entity = new Entity();
            entity.typeId = id;
            entity.components = new Dictionary<Identifier, IComponent>();

            // Instantiate and save behaviors
            foreach (var kvp in subject.components)
            {
                // Create copies of chains etc.
                entity.components.Add(kvp.Key, (IComponent)((ICopyable)kvp.Value).Copy());
            }

            return entity;
        }

        public void InitInWorld(Transform transform)
        {
            if (InitInWorldFunc != null) InitInWorldFunc(transform);
        }
        
        public static implicit operator Entity(EntityFactory factory) => factory.subject;
    }
}
