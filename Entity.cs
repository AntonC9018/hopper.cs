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

        public Acting beh_Acting
        {
            get
            {
                int id = Acting.s_factory.id;
                if (m_behaviors.ContainsKey(id))
                    return (Acting)m_behaviors[id];
                return null;
            }
        }
        public Sequenced beh_Sequenced
        {
            get
            {
                int id = Sequenced.s_factory.id;
                if (m_behaviors.ContainsKey(id))
                    return (Sequenced)m_behaviors[id];
                return null;
            }
        }
        public Attackable beh_Attackable
        {
            get
            {
                int id = Attackable.s_factory.id;
                if (m_behaviors.ContainsKey(id))
                    return (Attackable)m_behaviors[id];
                return null;
            }

        }
        public Attacking beh_Attacking
        {
            get
            {
                int id = Attacking.s_factory.id;
                if (m_behaviors.ContainsKey(id))
                    return (Attacking)m_behaviors[id];
                return null;
            }

        }

        public Pushable beh_Pushable
        {
            get
            {
                int id = Pushable.s_factory.id;
                if (m_behaviors.ContainsKey(id))
                    return (Pushable)m_behaviors[id];
                return null;
            }

        }

        public readonly Dictionary<int, Tinker> m_tinkers =
            new Dictionary<int, Tinker>();

        public Vector2 m_pos;
        public Vector2 m_orientation = Vector2.UnitX;
        public World m_world;

        internal List<Target> GetTargets(Action action)
        {
            return new List<Target>();
        }

        public Layer m_layer;

        // state
        // isDead is set to true when the entity needs to be filtered out 
        // and removed from the internal lists
        public bool b_isDead = false;

        public bool b_isPlayer = false;

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

        public List<Vector2> GetMovs()
        {
            return new List<Vector2>();
        }

        ~Entity()
        {
            foreach (var (_, tinker) in m_tinkers)
            {
                tinker.RemoveStore(id);
            }
        }


        public Entity GetClosestPlayer()
        {
            float minDist = 0;
            Entity closestPlayer = null;
            foreach (var player in m_world.m_state.m_players)
            {
                float curDist = (m_pos - player.m_pos).LengthSquared();

                if (closestPlayer == null || curDist < minDist)
                {
                    minDist = curDist;
                    closestPlayer = player;
                }
            }
            return closestPlayer;
        }

    }

    public class EntityFactory
    {
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();
        public Type entityClass;

        public Dictionary<string, ChainTemplate> chainTemplates =
            new Dictionary<string, ChainTemplate>();

        protected List<(BehaviorFactory, BehaviorConfig)> behaviorSettings =
            new List<(BehaviorFactory, BehaviorConfig)>();

        public void AddBehavior(BehaviorFactory factory, BehaviorConfig conf = null)
        {
            behaviorSettings.Add((factory, conf));
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
            foreach (var (behaviorFactory, conf) in behaviorSettings)
            {
                var behavior = behaviorFactory.Instantiate(entity, conf);
                entity.m_behaviors[behaviorFactory.id] = behavior;
            }
            return entity;
        }
    }
}