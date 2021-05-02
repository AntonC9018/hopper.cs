using System.Collections.Generic;
using Hopper.Core.Components.Basic;

namespace Hopper.Core
{
    public class WorldStateManager
    {
        public List<Acting>[] actings;

        public event System.Action StartOfLoopEvent;
        public event System.Action EndOfLoopEvent;
        public event System.Action BeforeFilterEvent;
        public event System.Action<Order> StartOfPhaseEvent;
        public event System.Action<Order> EndOfPhaseEvent;

        public Order currentPhase = Order.Player;
        private int m_currentTimeFrame = 0;
        public int NextTimeFrame() => m_currentTimeFrame++;
        private int[] m_updateCountPhaseLimits = new int[World.NumPhases];

        public WorldStateManager()
        {
            actings = new List<Acting>[World.NumPhases];

            for (int i = 0; i < actings.Length; i++)
            {
                actings[i] = new List<Acting>();
            }
        }

        public void AddActor(Acting acting)
        {
            actings[(int)acting.order].Add(acting);
        }

        public void Loop()
        {
            StartOfLoopEvent?.Invoke();

            ResetPhase();
            ActivatePlayers();
            AdvancePhase();
            CalculateActionOnEntities();
            ActivateOthers();
            TickAll();
            FilterDead();
            EndPhase();

            EndOfLoopEvent?.Invoke();
        }

        private void Activate(Acting acting)
        {
            if (acting.actor.IsDead()) return;
            if (!acting._flags.HasFlag(Acting.Flags.DidAction))
            {
                acting.Activate();
            }
        }

        private void CalculateActionOnEntities()
        {
            for (int i = (int)Order.Player + 1; i < actings.Length; i++)
            {
                foreach (var acting in actings[i])
                    acting.CalculateNextAction();
            }
        }

        private void ActivatePlayers()
        {
            for (int i = 0; i < actings[0].Count; i++)
            {
                Activate(actings[0][i]);
            }
        }

        private void ActivateOthers()
        {
            // skip the player, which is at 0.
            for (int i = (int)Order.Player + 1; i < actings.Length; i++)
            {
                for (int j = 0; j < actings[i].Count - 1; j--)
                    Activate(actings[i][j]);
                AdvancePhase();
            }
        }

        private void TickAll()
        {
            foreach (var @as in actings)
            {
                foreach (var acting in @as)
                    // TODO: tickings should be either stored in another list
                    //       and point to the entities too, or we should iterate directly
                    //       on the entities.
                    acting.actor.Tick();
            }
        }

        // TODO: filter the registry too? 
        // TODO: filter the ticking list too.
        // TODO: double buffer.
        private void FilterDead()
        {
            BeforeFilterEvent?.Invoke();
            for (int i = 0; i < actings.Length; i++)
            {
                var newActing = new List<Acting>();
                foreach (var acting in actings[i])
                {
                    if (acting.actor.IsDead())
                    {
                        Registry.Global.UnregisterRuntimeEntity(acting.actor);
                    }
                    else
                    {
                        newActing.Add(acting);
                    }
                }
                actings[i] = newActing;
            }
        }

        private void ResetPhase()
        {
            m_currentTimeFrame = 0;
            currentPhase = Order.Player;
        }

        private void AdvancePhase()
        {
            EndPhase();
            currentPhase++;
        }

        private void EndPhase()
        {
            m_updateCountPhaseLimits[(int)currentPhase] = m_currentTimeFrame;
            EndOfPhaseEvent?.Invoke(currentPhase);
        }

        public void OncePhaseStarts(Order phase, System.Action callback)
        {
            System.Action<Order> handler = null;
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

        public void OncePhaseEnds(Order phase, System.Action callback)
        {
            System.Action<Order> handler = null;
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