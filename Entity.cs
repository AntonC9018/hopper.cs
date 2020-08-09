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

        public Dictionary<string, Chain> m_chains =
            new Dictionary<string, Chain>();

        // the idea is to get the behavior instances like this:
        // entity.behaviors[Attackable.s_factory.id]
        public readonly Dictionary<int, Behavior> m_behaviors =
            new Dictionary<int, Behavior>();

        public Vector2 m_pos;
        public World m_world;

        public void AddChain(string name, Chain chain)
        {
            m_chains.Add(name, chain);
        }

        public void AddBehavior(int behaviorId, Behavior behavior)
        {
            m_behaviors.Add(behaviorId, behavior);
        }

        public Entity() { }

        public void Init(Vector2 pos)
        {

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
            foreach (var (name, template) in factory)
            {
                chainTemplates.Add(name, template);
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
                entity.AddChain(key, template.Init());
            }
            // Instantiate and save behaviors
            foreach (var behaviorFactory in behaviors)
            {
                var behavior = behaviorFactory.Instantiate(entity);
                entity.AddBehavior(behaviorFactory.id, behavior);
            }
            return entity;
        }
    }
}