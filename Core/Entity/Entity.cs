using System.Collections.Generic;
using Chains;
using Utils.Vector;
using Core.Behaviors;
using Core.Items;
using System;
using Utils;

namespace Core
{
    public class Entity : IProvideBehavior, IHaveId
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

        public IInventory Inventory { get; protected set; }
        public World World { get; private set; }
        public StatManager StatManager { get; private set; }
        public History History { get; private set; }

        // state
        // isDead is set to true when the entity needs to be filtered out 
        // and removed from the internal lists
        public bool IsDead { get; protected set; }

        // the idea is to get the behavior instances like this:
        // entity.behaviors[Attackable.s_factory.id]
        // Don't add stuff here. The contents of this are determined 
        // by the EntityFactory
        private readonly Dictionary<System.Type, Behavior> m_behaviors =
            new Dictionary<System.Type, Behavior>();

        // tinker's storage. The elements of this map are processed exclusively by tinkers
        // and the handlers added by them.
        private readonly Dictionary<int, TinkerData> m_tinkerStore =
            new Dictionary<int, TinkerData>();

        public Entity()
        {
            IsDead = false;
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



        public bool IsTinked(ITinker tinker)
        {
            return m_tinkerStore.ContainsKey(tinker.Id);
        }

        public void TinkAndSave(ITinker tinker)
        {
            if (IsTinked(tinker))
            {
                var store = m_tinkerStore[tinker.Id];
                store.count++;
            }
            else
            {
                m_tinkerStore[tinker.Id] = tinker.CreateDataAndTink(this);
            }
        }

        public void Untink(ITinker tinker)
        {
            var store = m_tinkerStore[tinker.Id];
            store.count--;
            if (store.count == 0)
            {
                tinker.Untink(store, this);
                m_tinkerStore.Remove(tinker.Id);
            }
        }

        public void TryUntink(ITinker tinker)
        {
            if (IsTinked(tinker))
                Untink(tinker);
        }

        public TinkerData GetTinkerStore(ITinker tinker)
        {
            return m_tinkerStore[tinker.Id];
        }

        // private void RetranslateEndOfLoopEvent()
        // {
        //     RetranslateEndOfLoopEvent?.
        // }
        // private void StartMonitoringEvents()
        // {
        //     World.m_state.EndOfLoopEvent += RetranslateEndOfLoopEvent;
        // }
        // public void StopMonitoringEvents()
        // {
        //     World.m_state.EndOfLoopEvent -= RetranslateEndOfLoopEvent;
        // }


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