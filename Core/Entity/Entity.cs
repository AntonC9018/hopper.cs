using System.Collections.Generic;
using Chains;
using Utils.Vector;
using Core.Behaviors;
using Core.Items;
using System;
using Utils;

namespace Core
{
    public class Entity : IHaveId
    {
        public int Id => m_id;
        private int m_id;

        protected IntVector2 m_pos;
        protected IntVector2 m_orientation = IntVector2.UnitX;
        public IntVector2 Pos { get => m_pos; set => m_pos = value; }
        public IntVector2 Orientation { get => m_orientation; }

        public virtual Layer Layer => Layer.REAL;
        public virtual bool IsPlayer => false;
        public Cell Cell => World.m_grid.GetCellAt(m_pos);

        // state
        // isDead is set to true when the entity needs to be filtered out 
        // and removed from the internal lists
        public bool IsDead { get; protected set; }

        public IInventory Inventory { get; protected set; }
        public World World { get; private set; }
        public StatManager StatManager { get; private set; }
        public History History { get; private set; }
        public BehaviorControl Behaviors { get; private set; }
        public TinkerControl Tinkers { get; private set; }

        public Entity()
        {
            IsDead = false;
            Behaviors = new BehaviorControl();
            Tinkers = new TinkerControl(this);
        }

        public void _SetId(int id)
        {
            this.m_id = id;
        }

        public void Init(IntVector2 pos, World world)
        {
            m_pos = pos;
            World = world;
            StatManager = new StatManager();
            History = new History();
            // StartMonitoringEvents();
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
    }

}