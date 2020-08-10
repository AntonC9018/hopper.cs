using System;
using System.Collections.Generic;
using Chains;
using System.Numerics;

namespace Core
{
    public class Entity
    {
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();

        // Don't add stuff here. The ontents of this are determined 
        // by the EntityFactory
        public readonly Dictionary<string, Chain> m_chains =
            new Dictionary<string, Chain>();

        // the idea is to get the behavior instances like this:
        // entity.behaviors[Attackable.s_factory.id]
        // Don't add stuff here. The contents of this are determined 
        // by the EntityFactory
        public readonly Dictionary<int, Behavior> m_behaviors =
            new Dictionary<int, Behavior>();

        public readonly Dictionary<int, Tinker> m_tinkers =
            new Dictionary<int, Tinker>();

        public Vector2 m_pos;
        public Vector2 m_orientation = Vector2.UnitX;
        public World m_world;

        public Entity()
        { }

        public void AddTinker(Tinker tinker)
        {
            m_tinkers[tinker.id] = tinker;
            tinker.Apply(this);
        }

        public void RemoveTinker(Tinker tinker)
        {
            m_tinkers.Remove(tinker.id);
            tinker.Remove(this);
        }

        public void Init(Vector2 pos, World world)
        {
            m_pos = pos;
            m_world = world;
        }

        ~Entity()
        {
            foreach (var (_, tinker) in m_tinkers)
            {
                tinker.RemoveStore(id);
            }
        }

    }

    public class EntityFactory
    {
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();
        public Type entityClass;

        public Dictionary<string, ChainTemplate> chainTemplates =
            new Dictionary<string, ChainTemplate>();

        protected List<BehaviorFactory> behaviors =
            new List<BehaviorFactory>();

        public void AddBehavior(BehaviorFactory factory)
        {
            behaviors.Add(factory);
            foreach (var chainTemplateDefinition in factory.m_chainTemplateDefinitions)
            {
                chainTemplates.Add(
                    chainTemplateDefinition.name,
                    chainTemplateDefinition.template);
            }
        }

        public void AddRetoucher(Retoucher retoucher)
        {

        }

        public Entity Instantiate()
        {
            Entity entity = (Entity)System.Activator.CreateInstance(entityClass);
            // Add all the chains
            foreach (var (key, template) in chainTemplates)
            {
                entity.m_chains[key] = template.Init();
            }
            // Instantiate and save behaviors
            foreach (var behaviorFactory in behaviors)
            {
                var behavior = behaviorFactory.Instantiate(entity);
                entity.m_behaviors[behaviorFactory.id] = behavior;
            }
            return entity;
        }
    }
}