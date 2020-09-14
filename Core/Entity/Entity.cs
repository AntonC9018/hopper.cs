using System.Collections.Generic;
using Chains;
using Vector;
using Core.Behaviors;
using Core.Items;
using System;

namespace Core
{
    public class Entity : IProvideBehavior
    {
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();

        protected IntVector2 m_pos;
        protected IntVector2 m_orientation = IntVector2.UnitX;
        public IntVector2 Pos { get => m_pos; set => m_pos = value; }
        public IntVector2 Orientation { get => m_orientation; }

        public virtual Layer Layer => Layer.REAL;
        public virtual bool IsPlayer => false;
        public Cell Cell => World.m_grid.GetCellAt(m_pos);

        public IInventory Inventory { get; protected set; }
        public World World { get; private set; }
        public StatManager StatManager { get; private set; }
        public History History { get; private set; }

        // state
        // isDead is set to true when the entity needs to be filtered out 
        // and removed from the internal lists
        public bool IsDead { get; protected set; }

        public Entity()
        {
            IsDead = false;
        }

        public void Init(IntVector2 pos, World world)
        {
            m_pos = pos;
            World = world;
            StatManager = new StatManager();
            History = new History();
            StartMonitoringEvents();
        }


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

        internal void Die()
        {
            IsDead = true;
            RemoveFromGrid();
        }
        // the idea is to get the behavior instances like this:
        // entity.behaviors[Attackable.s_factory.id]
        // Don't add stuff here. The contents of this are determined 
        // by the EntityFactory
        private readonly Dictionary<System.Type, Behavior> m_behaviors =
            new Dictionary<System.Type, Behavior>();

        // A setup method. May also be used at runtime, but setting up
        // behaviors in factory is prefered.
        internal void AddBehavior(Type t, Behavior behavior)
        {
            m_behaviors[t] = behavior;
        }

        public T GetBehavior<T>() where T : Behavior, new()
        {
            if (m_behaviors.ContainsKey(typeof(T)))
                return (T)m_behaviors[typeof(T)];
            return null;
        }

        public bool HasBehavior<T>() where T : Behavior, new()
        {
            return m_behaviors.ContainsKey(typeof(T));
        }

        private readonly Dictionary<int, ITinker> m_tinkers =
            new Dictionary<int, ITinker>();

        public bool IsTinked(ITinker tinker)
        {
            return m_tinkers.ContainsKey(tinker.id);
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
            foreach (var tinker in m_tinkers.Values)
            {
                tinker.RemoveStore(id);
            }
        }

        private void RetranslateEndOfLoopEvent()
        {
            Tick.chain.ChainPath(this)
                .Pass(new Tick.Event { actor = this });
        }
        private void StartMonitoringEvents()
        {
            World.m_state.EndOfLoopEvent += RetranslateEndOfLoopEvent;
        }
        public void StopMonitoringEvents()
        {
            World.m_state.EndOfLoopEvent -= RetranslateEndOfLoopEvent;
        }


        public Entity GetClosestPlayer()
        {
            float minDist = 0;
            Entity closestPlayer = null;
            foreach (var player in World.m_state.m_players)
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
    }

}