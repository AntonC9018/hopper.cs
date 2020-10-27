using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Behaviors;
using Core.Utils;

namespace Core
{
    public class WorldStateManager
    {
        private List<Entity>[] m_entities
            = new List<Entity>[World.NumLayers];

        // note that players never get into the lists from above
        private List<Entity> m_players = new List<Entity>();

        public ReadOnlyCollection<Entity> Players => m_players.AsReadOnly();
        public ReadOnlyCollection<List<Entity>> Entities => System.Array.AsReadOnly(m_entities);

        // Subscribe to this event only from the entity
        // This easily prevents memory leaks. How?
        // The handlers added to this event are strong pointers.
        // This event is replicated on the entity, to control this.
        public event System.Action EndOfLoopEvent;
        public event System.Action BeforeFilterEvent;

        private Phase m_currentPhase = Phase.PLAYER;
        public Phase CurrentPhase => m_currentPhase;

        private int m_iterCount = 0;
        public int InterationCount => m_iterCount;

        private int m_currentTimeFrame = 0;
        public int GetNextTimeFrame() => m_currentTimeFrame++;

        private int[] m_timeStampPhaseLimits = new int[World.NumPhases];
        public ReadOnlyCollection<int> TimeStampPhaseLimit
            => System.Array.AsReadOnly(m_timeStampPhaseLimits);

        public WorldStateManager()
        {
            for (int i = 0; i < m_entities.Length; i++)
            {
                m_entities[i] = new List<Entity>();
            }
        }

        public void AddEntity(Entity entity)
        {
            m_entities[entity.Layer.ToIndex()].Add(entity);
        }

        public int AddPlayer(Entity player)
        {
            m_players.Add(player);
            return m_players.Count - 1;
        }

        public void Loop()
        {
            ResetPhase();
            ClearHistory();
            ActivatePlayers();
            CalculateActionOnEntities();
            ActivateEntities();
            TickAll();
            FilterDead();
            EndPhase();

            EndOfLoopEvent?.Invoke();
            m_iterCount++;
        }

        private void ClearHistory()
        {
            foreach (var player in m_players)
            {
                player.History.Clear();
            }
            for (int i = 0; i < m_entities.Length; i++)
            {
                foreach (var e in m_entities[i])
                    e.History.Clear();
            }
        }

        private void CalculateNextAction(Entity entity)
        {
            entity.Behaviors.Get<Acting>()?.CalculateNextAction();
        }

        private void Activate(Entity entity)
        {
            if (entity.IsDead) return;
            var acting = entity.Behaviors.Get<Acting>();
            if (acting != null && !acting.b_didAction)
            {
                acting.Activate();
            }
        }

        private void ActivatePlayers()
        {
            foreach (var player in m_players)
            {
                Activate(player);
            }
            AdvancePhase();
        }

        private void CalculateActionOnEntities()
        {
            // Ideally, the acting decorators should be at one location in memory
            for (int i = 0; i < m_entities.Length; i++)
                foreach (var e in m_entities[i])
                    CalculateNextAction(e);
        }

        private void ActivateEntities()
        {
            for (int i = 0; i < m_entities.Length; i++)
            {
                AdvancePhase();
                foreach (var e in m_entities[i])
                    Activate(e);
            }
        }

        private void TickAll()
        {
            SetPhase(Phase.TICK_PLAYER);
            foreach (var player in m_players)
            {
                player.Behaviors.Get<Tick>().Activate();
            }
            SetPhase(Phase.TICK_REAL);
            foreach (var es in m_entities)
            {
                foreach (var e in es)
                    e.Behaviors.Get<Tick>().Activate();
            }
        }

        private void FilterDead()
        {
            BeforeFilterEvent?.Invoke();
            for (int i = 0; i < m_entities.Length; i++)
            {
                var newEntities = new List<Entity>();
                foreach (var entity in m_entities[i])
                {
                    if (entity.IsDead)
                    {
                        IdMap.Entity.Remove(entity.Id);
                    }
                    else
                    {
                        newEntities.Add(entity);
                    }
                }
                m_entities[i] = newEntities;
            }
        }

        private void ResetPhase()
        {
            m_currentTimeFrame = 0;
            m_currentPhase = Phase.PLAYER;
        }

        private void AdvancePhase()
        {
            EndPhase();
            m_currentPhase++;
        }

        private void SetPhase(Phase phase)
        {
            EndPhase();
            m_currentPhase = phase;
        }

        private void EndPhase()
        {
            m_timeStampPhaseLimits[(int)m_currentPhase] = m_currentTimeFrame;
        }
    }
}