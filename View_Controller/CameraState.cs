using System.Collections.Generic;
using Hopper.Core;
using Hopper.Core.History;
using Hopper.Utils.Vector;

namespace Hopper.Controller
{
    public class CameraState
    {
        public Vector2 m_prevCamPos;
        public Vector2[] m_posByPhase;

        private int m_count;

        public CameraState(IReadOnlyList<Entity> players)
        {
            m_prevCamPos = GetCenterBetweenPlayers(players);
            m_posByPhase = null;
        }

        public void NextLoop()
        {
            m_posByPhase = CreateCameraDataArray();
            m_count = 0;
        }

        private Vector2 GetCenterBetweenPlayers(IReadOnlyList<Entity> players)
        {
            var sum = new Vector2(0, 0);
            foreach (var player in players)
            {
                sum += player.Pos;
            }
            return sum;
        }

        private Vector2[] CreateCameraDataArray()
        {
            var arr = new Vector2[World.NumPhases];
            for (int i = 0; i < World.NumPhases; i++)
            {
                arr[i] = new Vector2(0, 0);
            }
            return arr;
        }

        public void AccumulateStates(EntityState[] states)
        {
            for (int i = 0; i < m_posByPhase.Length; i++)
            {
                m_posByPhase[i] = m_posByPhase[i] + states[i].pos;
            }
            m_count++;
        }

        public void AverageOutStates()
        {
            for (int i = 0; i < m_posByPhase.Length; i++)
            {
                m_posByPhase[i] /= m_count;
            }
        }

        public void EnterState(int currentPhase)
        {
            if (currentPhase > 0)
            {
                m_prevCamPos = m_posByPhase[currentPhase - 1];
            }
        }

        public Vector2 GetTransitionVector(int currentPhase)
        {
            return m_posByPhase[currentPhase] - m_prevCamPos;
        }
    }
}