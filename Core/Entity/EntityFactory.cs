using Hopper.Core.Registries;
using System.Collections.Generic;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stats;

namespace Hopper.Core
{
    public class EntityFactory
    {
        public Entity subject;
        public event System.Action<Entity> InitEvent;
        public event System.Action<PatchArea> PostPatchEvent;
        public DefaultStats DefaultStats;

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
            entity.components = new Dictionary<Identifier, IComponent>(subject.components);

            // Instantiate and save behaviors
            foreach (var kvp in entity.components)
            {
                // Create copies of chains etc.
                kvp.Value.CopyObjects();
            }

            InitEvent?.Invoke(entity);
            return entity;
        }

        public void PostPatch(PatchArea patchArea)
        {
            PostPatchEvent?.Invoke(patchArea);
            PostPatchEvent = null;
        }

        public EntityFactory Retouch(Retoucher retoucher)
        {
            return this;
        }

        public EntityFactory AddSetupListener(System.Action<Entity> listener)
        {
            InitEvent += listener;
            return this;
        }
    }
}
