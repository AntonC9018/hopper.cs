using System.Collections.Generic;
using Chains;
using Vector;
using Core.Behaviors;
using Core.Items;

namespace Core
{
    public class Entity : IProvideBehavior
    {
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();


        internal void Reorient(IntVector2 direction)
        {
            this.m_orientation = direction;
        }

        // the idea is to get the behavior instances like this:
        // entity.behaviors[Attackable.s_factory.id]
        // Don't add stuff here. The contents of this are determined 
        // by the EntityFactory
        public readonly Dictionary<System.Type, Behavior> m_behaviors =
            new Dictionary<System.Type, Behavior>();

        public T GetBehavior<T>() where T : Behavior, new()
        {
            if (m_behaviors.ContainsKey(typeof(T)))
                return (T)m_behaviors[typeof(T)];
            return null;
        }

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
            Tick.chain.ChainPath(this)
                .Pass(new Tick.Event { actor = this });
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
        public void AddBehavior<Beh>(BehaviorConfig conf)
            where Beh : Behavior, new();
        public void RetouchAndSave(Retoucher retoucher);
        public bool IsRetouched(Retoucher retoucher);
    }

    public class EntityFactory<T> : IEntityFactory, IProvideBehaviorFactory
        where T : Entity, new()
    {
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();
        public event System.Action<Entity> InitEvent;

        public EntityFactory()
        {
            AddBehavior<Tick>();
        }

        protected Dictionary<System.Type, (BehaviorFactory, BehaviorConfig)> m_behaviorSettings =
            new Dictionary<System.Type, (BehaviorFactory, BehaviorConfig)>();

        protected Dictionary<int, Retoucher> m_retouchers =
            new Dictionary<int, Retoucher>();

        public void AddBehavior<Beh>(BehaviorConfig conf = null)
            where Beh : Behavior, new()
        {
            var factory = new BehaviorFactory<Beh>();
            m_behaviorSettings.Add(typeof(Beh), (factory, conf));
        }

        public void RetouchAndSave(Retoucher retoucher)
        {
            m_retouchers.Add(retoucher.id, retoucher);
            retoucher.Retouch(this);
        }

        public bool IsRetouched(Retoucher retoucher)
        {
            return m_retouchers.ContainsKey(retoucher.id);
        }

        public Entity Instantiate()
        {
            Entity entity = new T();
            // Instantiate and save behaviors
            foreach (var (t, (behaviorFactory, conf)) in m_behaviorSettings)
            {
                var behavior = behaviorFactory.Instantiate(entity, conf);
                entity.m_behaviors[t] = behavior;
            }
            InitEvent?.Invoke(entity);
            return entity;
        }

        public BehaviorFactory<U> GetBehaviorFactory<U>() where U : Behavior, new()
        {
            return (BehaviorFactory<U>)m_behaviorSettings[typeof(U)].Item1;
        }
    }
}