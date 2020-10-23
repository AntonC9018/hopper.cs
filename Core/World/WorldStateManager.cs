using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Behaviors;
using Utils;

namespace Core
{
    public class WorldStateManager
    {
        private List<Entity>[] m_entities
            = new List<Entity>[World.s_numEntityTypes];

        // note that players never get into the lists from above
        private List<Entity> m_players = new List<Entity>();

        public IReadOnlyList<Entity> Players => m_players.AsReadOnly();
        public ReadOnlyCollection<List<Entity>> Entities => System.Array.AsReadOnly(m_entities);

        // Subscribe to this event only from the entity
        // This easily prevents memory leaks. How?
        // The handlers added to this event are strong pointers.
        // This event is replicated on the entity, to control this.
        public event System.Action EndOfLoopEvent;
        public event System.Action BeforeFilterEvent;

        public int m_phase = 0;
        public int m_numIters = 0;

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
            ActivatePlayers();
            CalculateActionOnEntities();
            ActivateEntities();
            TickAll();
            FilterDead();

            EndOfLoopEvent?.Invoke();
            m_numIters++;
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
                m_phase = i;
                foreach (var e in m_entities[i])
                    Activate(e);
            }
            m_phase = 0;
        }

        private void TickAll()
        {
            foreach (var player in m_players)
            {
                player.Behaviors.Get<Tick>().Activate();
            }
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
                m_entities[i] = m_entities[i].Where(e => e.IsDead == false);
            }
        }
    }
}