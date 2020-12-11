using System.Collections.Generic;
using Hopper.Core.Behaviors;

namespace Hopper.Core
{
    public class WorldStateManager
    {
        private List<Entity>[] m_entities
            = new List<Entity>[World.NumLayers];

        // note that players never get into the lists from above
        private List<Entity> m_players = new List<Entity>();

        public IReadOnlyList<Entity> Players => m_players.AsReadOnly();
        public IReadOnlyList<List<Entity>> Entities => System.Array.AsReadOnly(m_entities);

        public event System.Action StartOfLoopEvent;
        public event System.Action EndOfLoopEvent;
        public event System.Action BeforeFilterEvent;
        public event System.Action<Phase> StartOfPhaseEvent;
        public event System.Action<Phase> EndOfPhaseEvent;

        private Phase m_currentPhase = Phase.PLAYER;
        public Phase CurrentPhase => m_currentPhase;

        private int m_iterCount = 0;
        public int InterationCount => m_iterCount;

        private int m_currentTimeFrame = 0;
        public int GetNextTimeFrame() => m_currentTimeFrame++;

        private int[] m_updateCountPhaseLimits = new int[World.NumPhases];
        public IReadOnlyList<int> UpdateCountPhaseLimit
            => System.Array.AsReadOnly(m_updateCountPhaseLimits);

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
            StartOfLoopEvent?.Invoke();

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
            entity.Behaviors.TryGet<Acting>()?.CalculateNextAction();
        }

        private void Activate(Entity entity)
        {
            if (entity.IsDead) return;
            var acting = entity.Behaviors.TryGet<Acting>();
            if (acting != null && !acting.DidAction)
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
            for (int i = 0; i < m_entities.Length; i++)
            {
                foreach (var e in m_entities[i])
                    CalculateNextAction(e);
            }
        }

        private void ActivateEntities()
        {
            for (int i = 0; i < m_entities.Length; i++)
            {
                for (int j = m_entities[i].Count - 1; j >= 0; j--)
                    Activate(m_entities[i][j]);
                AdvancePhase();
            }
        }

        private void TickAll()
        {
            // SetPhase(Phase.TICK_PLAYER);
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
                        // TODO: Store entities by world
                        // Registry.Default.Entity.Remove(entity.Id);
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
            m_updateCountPhaseLimits[(int)m_currentPhase] = m_currentTimeFrame;
            EndOfPhaseEvent?.Invoke(m_currentPhase);
        }

        public void OncePhaseStarts(Phase phase, System.Action callback)
        {
            System.Action<Phase> handler = null;
            handler = (p) =>
            {
                if (p == phase)
                {
                    callback();
                    StartOfPhaseEvent -= handler;
                }
            };
            StartOfPhaseEvent += handler;
        }

        public void OncePhaseEnds(Phase phase, System.Action callback)
        {
            System.Action<Phase> handler = null;
            handler = (p) =>
            {
                if (p == phase)
                {
                    callback();
                    EndOfPhaseEvent -= handler;
                }
            };
            EndOfPhaseEvent += handler;
        }
    }
}