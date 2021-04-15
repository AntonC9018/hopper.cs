using System.Collections.Generic;
using Hopper.Core.Components;

namespace Hopper.Core
{
    public class EntityFactory
    {
        public Identifier id;
        public Entity subject;

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
            Entity entity = new Entity(); // subject.Clone()?
            entity.components = new Dictionary<Identifier, IComponent>();

            // Instantiate and save behaviors
            foreach (var kvp in subject.components)
            {
                // Create copies of chains etc.
                entity.components.Add(kvp.Key, (IComponent) kvp.Value.Copy());
            }

            return entity;
        }
        
        public static implicit operator Entity(EntityFactory factory) => factory.subject;
    }
}
