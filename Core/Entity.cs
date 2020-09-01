using System.Collections.Generic;
using Chains;
using Vector;
using Core.Behaviors;
using Core.Items;

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
        public readonly Dictionary<int, IBehavior> m_behaviors =
            new Dictionary<int, IBehavior>();

        public IBehavior GetBehavior(int id)
        {
            if (m_behaviors.ContainsKey(id))
                return m_behaviors[id];
            return null;
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
        public Statused beh_Statused
        { get { return (Statused)GetBehavior(Statused.s_factory.id); } }
        public Dictionary<int, IStatus> AppliedStatuses
        { get { return beh_Statused?.m_activeStatuses; } }

        public readonly Dictionary<int, ITinker> m_tinkers =
            new Dictionary<int, ITinker>();

        public IntVector2 m_pos;
        public IntVector2 m_orientation = IntVector2.UnitX;
        public World m_world;
        public History m_history;
        public virtual Layer Layer { get => Layer.REAL; }
        public virtual IInventory Inventory { get; set; } // for now, get set. however, this should be readonly

        // TODO: we need a stat manager factory with the ability to set up 
        // default stats.
        public StatManager m_statManager = new StatManager();

        // state
        // isDead is set to true when the entity needs to be filtered out 
        // and removed from the internal lists
        public bool b_isDead = false;
        public virtual bool IsPlayer { get => false; }

        public Entity()
        { }

        public void Init(IntVector2 pos, World world)
        {
            m_history = new History();
            m_pos = pos;
            m_world = world;
            StartMonitoringEvents();
        }

        public void TinkAndSave(ITinker tinker)
        {
            m_tinkers[tinker.id] = tinker;
            tinker.Tink(this);
        }
        public void Untink(ITinker tinker)
        {
            m_tinkers.Remove(tinker.id);
            tinker.Untink(this);
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

        public Cell Cell => m_world.m_grid.GetCellAt(m_pos);

        public Entity GetClosestPlayer()
        {
            float minDist = 0;
            Entity closestPlayer = null;
            foreach (var player in m_world.m_state.m_players)
            {
                float curDist = (m_pos - player.m_pos).SqMag;

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

    public interface IEntityFactory
    {
        public Entity Instantiate();
    }

    public class EntityFactory<T> : IEntityFactory where T : Entity, new()
    {
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();
        public event System.Action<Entity> InitEvent;

        public EntityFactory()
        {
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
            foreach (var (name, template) in factory.Templates)
                m_chainTemplates.Add(name, template);
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
            Entity entity = new T();
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