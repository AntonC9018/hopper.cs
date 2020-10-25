using Utils.Vector;
using Core.Items;
using System.Runtime.Serialization;
using Core.Stats;
using Core.Targeting;
using Core.History;

namespace Core
{
    [DataContract]
    public class Entity : IHaveId, IWorldSpot, ITrackable<EntityState>
    {
        [DataMember] public int Id => m_id;
        private int m_id;

        [DataMember(Name = "pos")]
        protected IntVector2 m_pos;
        [DataMember(Name = "orientation")]
        protected IntVector2 m_orientation = IntVector2.UnitX;
        public IntVector2 Pos { get => m_pos; set => m_pos = value; }
        public IntVector2 Orientation { get => m_orientation; }

        public virtual Layer Layer => Layer.REAL;
        public virtual bool IsPlayer => false;
        public Cell Cell => World.m_grid.GetCellAt(m_pos);

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
        public StatManager Stats { get; private set; }

        public History<EntityState> History { get; private set; }

        EntityState ITrackable<EntityState>.GetState()
        {
            return new EntityState(this);
        }

        public Entity()
        {
            IsDead = false;
            Behaviors = new BehaviorControl();
            Tinkers = new TinkerControl(this);
            Stats = new StatManager();
            History = new History<EntityState>();
        }

        public void _SetId(int id)
        {
            this.m_id = id;
        }

        public void Init(IntVector2 pos, World world)
        {
            m_pos = pos;
            World = world;
        }

        public void Init(World world)
        {
            World = world;
        }

        // Utility methods
        public void ResetPosInGrid(IntVector2 newPos)
        {
            RemoveFromGrid();
            m_pos = newPos;
            ResetInGrid();
        }

        public void RemoveFromGrid()
        {
            World.m_grid.Remove(this);
        }

        public void ResetInGrid()
        {
            World.m_grid.Reset(this);
        }

        public void Reorient(IntVector2 orientation)
        {
            m_orientation = orientation;
        }

        public void Die()
        {
            IsDead = true;
            RemoveFromGrid();
            IdMap.Entity.Remove(m_id);
        }

        public Entity GetClosestPlayer()
        {
            float minDist = 0;
            Entity closestPlayer = null;
            foreach (var player in World.m_state.Players)
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

        public IntVector2 GetPosRelative(IntVector2 offset)
        {
            return m_pos + offset;
        }

        public Cell GetCellRelative(IntVector2 offset)
        {
            return World.m_grid.GetCellAt(m_pos + offset);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode() => m_id;

        public int GetFactoryId()
        {
            return IdMap.Entity.MapMetadata(m_id).factoryId;
        }
    }
}