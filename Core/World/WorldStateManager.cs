using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hopper.Core.ActingNS;
using Hopper.Core.Components.Basic;
using Hopper.Utils;

namespace Hopper.Core.WorldNS
{
    public class WorldStateManager
    {
        public readonly DoubleList<Acting>[] _allActings;
        public readonly DoubleList<Ticking> _allTickings;

        public Phase currentPhase = Phase.Done;

        public WorldStateManager()
        {
            _allActings = new DoubleList<Acting>[World.NumOrders];

            for (int i = 0; i < _allActings.Length; i++)
            {
                _allActings[i] = new DoubleList<Acting>();
            }

            _allTickings = new DoubleList<Ticking>();
        }

        public void AddActor(Acting acting)
        {
            _allActings[(int)acting.order].AddMaybeWhileIterating(acting);
        }

        public void AddTicking(Ticking ticking)
        {
            _allTickings.AddMaybeWhileIterating(ticking);
        }

        public void Loop()
        {
            LoopCoroutine().Consume();

            // ResetPhase();
            // ActivatePlayers();
            // CalculateActionOnEntities();
            // ActivateOthers();
            // TickAll();
            // FilterDead();
            // EndPhase();
        }

        /// <summary>
        /// Does a controlled iteration by phases, in coroutine fashion.
        /// Doing a foreach on this function is equivalent to calling <c>Loop()</c>.
        /// You may use <c>MoveNext()</c> and <c>Current</c> to skip the individual elements.
        /// The phase (order) value yield returned by the function indicates the next phase,
        /// so consuming the next value would execute that phase.
        /// </summary>
        public IEnumerable<Phase> LoopCoroutine()
        {
            // Check if this coroutine has not been started already
            Assert.That(currentPhase == Phase.Done, "Already doing the game loop!");

            currentPhase = Phase.Calculate_Actions;
            yield return currentPhase;
            CalculateActionOnEntities();

            currentPhase = Phase.Player_Act;
            yield return currentPhase;
            ActivatePlayers();
            currentPhase++;

            yield return currentPhase;
            foreach (var phase in ActivateOthers())
            {
                // phase here is the same as current phase.
                yield return phase;
            }
            
            Assert.That(currentPhase == Phase.Ticking);
            TickAll();

            currentPhase = Phase.FilterDead;
            yield return currentPhase;
            FilterDead();

            currentPhase = Phase.Done;
        }

        private void Activate(Acting acting)
        {
            if (!acting.actor.IsDead()) 
            {
                acting.ActivateIfDidNotAct();
            }
        }

        private void CalculateActionOnEntities()
        {
            foreach (var actingsOfSomeOrder in _allActings)
            foreach (var acting in actingsOfSomeOrder)
                acting.CalculateAndSetAction();
        }

        private void ActivatePlayers()
        {
            foreach (var acting in _allActings[0])
            {
                Activate(acting);
            }
        }

        private IEnumerable<Phase> ActivateOthers()
        {
            // skip the player, which is at 0.
            foreach (var actingsOfSomeOrder in _allActings.Skip(1))
            {
                foreach (var acting in actingsOfSomeOrder)
                {
                    Activate(acting);
                }
                yield return ++currentPhase;
            }
        }

        private void TickAll()
        {
            foreach (var ticking in _allTickings.StartFiltering())
            {
                if (!ticking.actor.IsDead())
                {
                    ticking.Activate();
                    _allTickings.AddToSecondaryBuffer(ticking);
                }
            }
        }

        private void FilterDead()
        {
            foreach (var actingsOfSomeOrder in _allActings)
            foreach (var acting in actingsOfSomeOrder.StartFiltering())
            {
                if (!acting.actor.IsDead())
                {
                    actingsOfSomeOrder.AddToSecondaryBuffer(acting);
                }
            }
        }
    }
}
