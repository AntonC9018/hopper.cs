using System.Collections.Generic;
using System.Linq;
using Hopper.Core;

namespace Hopper.Controller
{
    public delegate void TimerEventHandler(AnimationInfo info);

    public class ViewAnimator : IAnimator
    {
        private ICamera m_camera;
        private ITimer m_timer;
        private IEnumerable<HistoryData> m_currentData;

        private CameraState m_cameraState;

        // TODO: think about the overlaying phases     
        // TODO: think about skipping phases without any updates or slurring ones
        private int[] m_phaseSpanMillis;
        private int m_totalTimePerIteration;
        private int m_currentPhase;
        private int m_tickCount;

        public ViewAnimator(ICamera camera, ITimer timer)
        {
            m_camera = camera;
            m_timer = timer;
            m_timer.TimerEvent += Tick;

            m_phaseSpanMillis = new int[World.NumPhases];
            // for now, leave the rest at 0
            m_phaseSpanMillis[(int)Phase.PLAYER] = 300;
            // m_phaseSpanMillis[(int)Phase.REAL] = 300;
            m_phaseSpanMillis[(int)Phase.TRAP] = 300;

            m_totalTimePerIteration = m_phaseSpanMillis.Sum();
        }

        public void SetCamera(CameraState cameraState)
        {
            m_camera.SetInitialPosition(cameraState.m_prevCamPos);
            m_cameraState = cameraState;
        }

        public void Animate(IEnumerable<HistoryData> historyData)
        {
            m_currentPhase = 0;
            m_tickCount = 0;
            m_firstTimeThisPhase = true;

            m_currentData = historyData;

            m_timer.Start();
        }

        private bool m_firstTimeThisPhase;

        private void Tick(int millis)
        {
            float proportionIntoPhase = System.Math.Min(CalculateProportionIntoPhase(millis), 1);
            var animationInfo = new AnimationInfo
            {
                currentPhase = m_currentPhase,
                proportionIntoPhase = proportionIntoPhase,
                tickCount = m_tickCount
            };

            // potentially call a transition state on scene ents
            if (m_firstTimeThisPhase)
            {
                m_cameraState.EnterState(m_currentPhase);
            }

            foreach (var data in m_currentData)
            {
                if (m_firstTimeThisPhase)
                {
                    var state = data.entityStatesAndSieves.states[m_currentPhase];
                    var sieve = data.entityStatesAndSieves.sieves[m_currentPhase];
                    data.sceneEnt.EnterPhase(state, sieve, animationInfo);
                }
                else
                {
                    data.sceneEnt.Update(animationInfo);
                }
            }

            var cameraDelta = m_cameraState.GetTransitionVector(m_currentPhase) * proportionIntoPhase;
            m_camera.ChangePos(m_cameraState.m_prevCamPos + cameraDelta);

            if (IsPastLastPhase(millis))
            {
                m_timer.Stop();
            }
            else
            {
                m_firstTimeThisPhase = TryAdvancePhase(millis);
            }
        }

        private float CalculateProportionIntoPhase(int millis)
        {
            return ((float)millis) / m_phaseSpanMillis[m_currentPhase];
        }

        private bool TryAdvancePhase(int millis)
        {
            if (m_currentPhase < World.NumPhases - 1
                && m_phaseSpanMillis[m_currentPhase] < millis)
            {
                millis -= m_phaseSpanMillis[m_currentPhase];
                m_currentPhase++;
                m_timer.Reset();
                return true;
            }
            return false;
        }

        private bool IsPastLastPhase(int millis)
        {
            return m_currentPhase == World.NumPhases - 1
                && millis > m_phaseSpanMillis[m_currentPhase];
        }
    }
}