using Hopper.Core.Registries;
using System.Collections.Generic;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stats;

namespace Hopper.Core
{
    public class EntityFactory : Entity
    {
        public event System.Action<Entity> InitEvent;
        public event System.Action<PatchArea> PostPatchEvent;
        public DefaultStats DefaultStats;

        public EntityFactory()
        {
            components = new Dictionary<Identifier, IComponent>();
        }

        public Entity Instantiate()
        {
            Entity entity = new Entity();
            entity.components = new Dictionary<Identifier, IComponent>(components);

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
