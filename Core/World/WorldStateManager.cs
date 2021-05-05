using System.Linq;
using Hopper.Core.Components.Basic;
using Hopper.Utils;

namespace Hopper.Core
{

    public class WorldStateManager
    {
        public readonly DoubleList<Acting>[] actings;
        public DoubleList<Ticking> tickings;

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
            actings = new DoubleList<Acting>[World.NumPhases];

            for (int i = 0; i < actings.Length; i++)
            {
                actings[i] = new DoubleList<Acting>();
            }

            tickings = new DoubleList<Ticking>();
        }

        public void AddActor(Acting acting)
        {
            actings[(int)acting.order].AddMaybeWhileIterating(acting);
        }

        public void AddTicking(Ticking ticking)
        {
            tickings.AddMaybeWhileIterating(ticking);
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
            foreach (var acting in actings[0])
            {
                Activate(acting);
            }
        }

        private void ActivateOthers()
        {
            // skip the player, which is at 0.
            foreach (var _actings in actings.Skip(1))
            {
                foreach (var acting in _actings)
                {
                    Activate(acting);
                }
                AdvancePhase();
            }
        }

        private void TickAll()
        {
            foreach (var ticking in tickings.StartFiltering())
            {
                if (!ticking.actor.IsDead())
                {
                    ticking.Activate();
                    tickings.AddToSecondaryBuffer(ticking);
                }
            }
        }

        private void FilterDead()
        {
            BeforeFilterEvent?.Invoke();
            for (int i = 0; i < actings.Length; i++)
            {
                foreach (var acting in actings[i].StartFiltering())
                {
                    if (acting.actor.IsDead())
                    {
                        actings[i].AddToSecondaryBuffer(acting);
                    }
                }
            }
            // TODO: Filter the registry too
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
    }
}