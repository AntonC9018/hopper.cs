using System.Collections.Generic;
using Hopper.Core.Components.Basic;
using Hopper.Core.Registries;
using Hopper.Utils;

namespace Hopper.Core
{
    public class WorldStateManager
    {
        public List<Entity>[] entities = new List<Entity>[World.NumLayers];

        // note that players never get into the lists from above
        public List<Entity> players = new List<Entity>();

        public event System.Action StartOfLoopEvent;
        public event System.Action EndOfLoopEvent;
        public event System.Action BeforeFilterEvent;
        public event System.Action<Phase> StartOfPhaseEvent;
        public event System.Action<Phase> EndOfPhaseEvent;

        public Phase currentPhase = Phase.PLAYER;

        public int InterationCount = 0;

        private int m_currentTimeFrame = 0;
        public int NextTimeFrame() => m_currentTimeFrame++;

        private int[] m_updateCountPhaseLimits = new int[World.NumPhases];
        public IReadOnlyList<int> UpdateCountPhaseLimit
            => System.Array.AsReadOnly(m_updateCountPhaseLimits);

        public WorldStateManager()
        {
            for (int i = 0; i < entities.Length; i++)
            {
                entities[i] = new List<Entity>();
            }
        }

        public void AddEntity(Entity entity)
        {
            entities[entity.Layer.ToIndex()].Add(entity);
        }

        public int AddPlayer(Entity player)
        {
            players.Add(player);
            return players.Count - 1;
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
            foreach (var player in players)
            {
                player.History.Clear();
            }
            for (int i = 0; i < entities.Length; i++)
            {
                foreach (var e in entities[i])
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
            if (acting != null && !acting.HasDoneAction())
            {
                acting.Activate();
            }
        }

        private void ActivatePlayers()
        {
            foreach (var player in players)
            {
                Activate(player);
            }
            AdvancePhase();
        }

        private void CalculateActionOnEntities()
        {
            for (int i = 0; i < entities.Length; i++)
            {
                foreach (var e in entities[i])
                    CalculateNextAction(e);
            }
        }

        private void ActivateEntities()
        {
            for (int i = 0; i < entities.Length; i++)
            {
                for (int j = entities[i].Count - 1; j >= 0; j--)
                    Activate(entities[i][j]);
                AdvancePhase();
            }
        }

        private void TickAll()
        {
            // SetPhase(Phase.TICK_PLAYER);
            foreach (var player in players)
            {
                player.Behaviors.Get<Tick>().Activate();
            }
            SetPhase(Phase.TICK_REAL);
            foreach (var es in entities)
            {
                foreach (var e in es)
                    e.Behaviors.Get<Tick>().Activate();
            }
        }

        private void FilterDead()
        {
            BeforeFilterEvent?.Invoke();
            for (int i = 0; i < entities.Length; i++)
            {
                var newEntities = new List<Entity>();
                foreach (var entity in entities[i])
                {
                    if (entity.IsDead)
                    {
                        m_instanceSubregistry.Remove(entity.Id);
                    }
                    else
                    {
                        newEntities.Add(entity);
                    }
                }
                entities[i] = newEntities;
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