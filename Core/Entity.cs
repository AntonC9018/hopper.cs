using System;
using System.Collections.Generic;
using Chains;
using Vector;

namespace Core
{
    public class Entity
    {
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();


        internal void Reorient(IntVector2 direction)
        {
            this.m_orientation = direction;
        }

        // Don't add stuff here. The ontents of this are determined 
        // by the EntityFactory
        public readonly Dictionary<string, IChain> m_chains =
            new Dictionary<string, IChain>();

        // the idea is to get the behavior instances like this:
        // entity.behaviors[Attackable.s_factory.id]
        // Don't add stuff here. The contents of this are determined 
        // by the EntityFactory
        public readonly Dictionary<int, Behavior> m_behaviors =
            new Dictionary<int, Behavior>();

        public Behavior GetBehavior(int id)
        {
            if (m_behaviors.ContainsKey(id))
                return m_behaviors[id];
            return null;
        }

        public bool IsPlayer()
        {
            return false;
        }

        /*
        although this is boilerplate, it simplifies the process of reaching out
        to these behaviors from other parts of the program
        now obviously this is just shortcuts for all the predefined behaviors
        and isn't scalable. For other decorators one has to use
        `(*Behavior*)GetBehavior(*Behavior*.s_factory.id)`
        since s_factory is a different static property for each of the classes,
        inheriting Behavior, this cannot be easily automated. If it weren't, I'd do
        `GetBehavior<*Behavior*>` which would return the desired behavior, which would be sweet
        */
        public Acting beh_Acting
        { get { return (Acting)GetBehavior(Acting.s_factory.id); } }
        public Sequential beh_Sequenced
        { get { return (Sequential)GetBehavior(Sequential.s_factory.id); } }
        public Attackable beh_Attackable
        { get { return (Attackable)GetBehavior(Attackable.s_factory.id); } }
        public Attacking beh_Attacking
        { get { return (Attacking)GetBehavior(Attacking.s_factory.id); } }
        public Pushable beh_Pushable
        { get { return (Pushable)GetBehavior(Pushable.s_factory.id); } }
        public Displaceable beh_Displaceable
        { get { return (Displaceable)GetBehavior(Displaceable.s_factory.id); } }

        public readonly Dictionary<int, Tinker> m_tinkers =
            new Dictionary<int, Tinker>();

        public IntVector2 m_pos;
        public IntVector2 m_orientation = IntVector2.UnitX;
        public World m_world;
        public Layer m_layer = Layer.REAL;

        // TODO: we need a stat manager factory with the ability to set up 
        // default stats.
        public StatManager m_statManager = new StatManager();

        // state
        // isDead is set to true when the entity needs to be filtered out 
        // and removed from the internal lists
        public bool b_isDead = false;

        public bool b_isPlayer = false;

        public Entity()
        { }

        public void Init(IntVector2 pos, World world)
        {
            m_pos = pos;
            m_world = world;
            StartMonitoringEvents();
        }

        public void Tink(Tinker tinker)
        {
            m_tinkers[tinker.id] = tinker;
            tinker.Tink(this);
        }
        public void Untink(Tinker tinker)
        {
            m_tinkers.Remove(tinker.id);
            tinker.Untink(this);
        }
        // TODO:
        public void CreateDroppedItem(int id)
        {
        }
        // TODO: this won't work, since other tinkers may also reference
        // this entity in their stores.
        // I guess when that happens, they should be checking for whether
        // this object is dead or not to remove it when necessary.
        // We can't force the object cleanup otherwise.
        // If nothing eludes me, this is the only potential memory leak.
        ~Entity()
        {
            foreach (var (_, tinker) in m_tinkers)
            {
                tinker.RemoveStore(id);
            }
        }

        void RetranslateEndOfLoopEvent()
        {
            ((Chain<CommonEvent>)m_chains["tick"]).Pass(new CommonEvent { actor = this });
        }
        void StartMonitoringEvents()
        {
            m_world.m_state.EndOfLoopEvent += RetranslateEndOfLoopEvent;
        }
        public void StopMonitoringEvents()
        {
            m_world.m_state.EndOfLoopEvent -= RetranslateEndOfLoopEvent;
        }

        public Entity GetClosestPlayer()
        {
            float minDist = 0;
            Entity closestPlayer = null;
            foreach (var player in m_world.m_state.m_players)
            {
                float curDist = (m_pos - player.m_pos).SqMag();

                if (closestPlayer == null || curDist < minDist)
                {
                    minDist = curDist;
                    closestPlayer = player;
                }
            }
            return closestPlayer;
        }

        public IntVector2 GetRelativePos(IntVector2 offset)
        {
            return m_pos + offset;
        }

        public Cell GetCellRelative(IntVector2 offset)
        {
            return m_world.m_grid.GetCellAt(m_pos + offset);
        }

        public void RemoveFromGrid()
        {
            m_world.m_grid.Remove(this);
        }

        public void ResetInGrid()
        {
            m_world.m_grid.Reset(this);
        }

    }

    public class EntityFactory
    {
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();
        public Type m_entityClass;
        public event System.Action<Entity> InitEvent;

        public EntityFactory(System.Type entityClass)
        {
            m_entityClass = entityClass;
            m_chainTemplates.Add("tick", new ChainTemplate<CommonEvent>());
        }

        protected Dictionary<string, IChainTemplate> m_chainTemplates =
            new Dictionary<string, IChainTemplate>();

        protected List<(IBehaviorFactory, BehaviorConfig)> m_behaviorSettings =
            new List<(IBehaviorFactory, BehaviorConfig)>();

        protected Dictionary<int, Retoucher> m_retouchers =
            new Dictionary<int, Retoucher>();

        public void AddBehavior(IBehaviorFactory factory, BehaviorConfig conf = null)
        {
            m_behaviorSettings.Add((factory, conf));
            foreach (var chainTemplateDefinition in factory.m_chainTemplateDefinitions)
            {
                m_chainTemplates.Add(
                    chainTemplateDefinition.name,
                    chainTemplateDefinition.Template);
            }
        }

        public void AddRetoucher(Retoucher retoucher)
        {
            m_retouchers.Add(retoucher.id, retoucher);
            foreach (IChainDef cd in retoucher.chainDefinitions)
            {
                foreach (var handler in cd.handlers)
                    m_chainTemplates[cd.name].AddHandler(handler);
            }
        }

        public bool IsRetouched(Retoucher retoucher)
        {
            return m_retouchers.ContainsKey(retoucher.id);
        }

        public Entity Instantiate()
        {
            Entity entity = (Entity)System.Activator.CreateInstance(m_entityClass);
            // Add all the chains
            foreach (var (key, template) in m_chainTemplates)
            {
                entity.m_chains[key] = template.Init();
            }
            // Instantiate and save behaviors
            foreach (var (behaviorFactory, conf) in m_behaviorSettings)
            {
                var behavior = behaviorFactory.Instantiate(entity, conf);
                entity.m_behaviors[behaviorFactory.id] = behavior;
            }
            InitEvent?.Invoke(entity);
            return entity;
        }
    }
}