using Hopper.Utils.Vector;
using Hopper.Core.Items;
using System.Runtime.Serialization;
using Hopper.Core.Stats;
using Hopper.Core.History;

namespace Hopper.Core
{
    [DataContract]
    public class Entity : IHaveId, IWorldSpot, ITrackable<EntityState>
    {
        [DataMember] public int Id => m_id;
        private int m_id;

        [DataMember(Name = "pos")]
        protected IntVector2 m_pos = IntVector2.Zero;
        [DataMember(Name = "orientation")]
        protected IntVector2 m_orientation = IntVector2.Zero;
        public IntVector2 Pos { get => m_pos; set => m_pos = value; }
        public IntVector2 Orientation { get => m_orientation; set => m_orientation = value; }

        public virtual Layer Layer => Layer.REAL;
        public virtual bool IsDirected => false;
        public virtual bool IsPlayer => false;

        // state
        // isDead is set to true when the entity needs to be filtered out 
        // and removed from the internal lists
        [DataMember] public bool IsDead { get; protected set; }


        // These field store persistent state
        [DataMember] public IInventory Inventory { get; protected set; }

        [DataMember(Order = 1)] public BehaviorControl Behaviors { get; private set; }
        [DataMember(Order = 2)] public TinkerControl Tinkers { get; private set; }

        // These fields do not store persistent state
        public World World { get; private set; }
        public StatManager Stats { get; internal set; }

        public History<EntityState> History { get; private set; }

        // this event is fired once the entity gets assigned 
        // the world and its initial position in the world
        public event System.Action InitEvent;
        public event System.Action DieEvent;

        EntityState ITrackable<EntityState>.GetState() => new EntityState(this);

        public Entity()
        {
            IsDead = false;
            Behaviors = new BehaviorControl();
            Tinkers = new TinkerControl();
            History = new History<EntityState>();
        }

        internal void _SetId(int id)
        {
            this.m_id = id;
        }

        public void RegisterSelf(Registry registry)
        {
            throw new System.Exception("Entity cannot register self");
        }

        public void Init(IntVector2 pos, IntVector2 orientation, World world)
        {
            m_orientation = orientation;
            m_pos = pos;
            World = world;

            History.InitControlUpdate(this);

            if (Stats == null)
                Stats = new StatManager(world.m_currentRegistry);

            // fire the init event and destroy it, since it should only be called once
            InitEvent?.Invoke();
            InitEvent = null;
        }

        // Utility methods
        public void ResetPosInGrid(IntVector2 newPos)
        {
            RemoveFromGrid();
            World.Grid.Reset(this, newPos);
            m_pos = newPos;
        }

        public void RemoveFromGrid()
        {
            World.Grid.Remove(this);
        }

        public void ResetInGrid()
        {
            World.Grid.Reset(this, m_pos);
        }

        public void Reorient(IntVector2 orientation)
        {
            m_orientation = orientation;
        }

        public void Die()
        {
            IsDead = true;
            RemoveFromGrid();

            DieEvent?.Invoke();

            History.Add(this, UpdateCode.dead);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode() => m_id;

        public int GetFactoryId()
        {
            return World.m_currentRegistry.Entity.MapMetadata(m_id).factoryId;
        }
    }
}