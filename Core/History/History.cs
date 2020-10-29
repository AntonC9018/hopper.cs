using System.Collections.Generic;
using System.Linq;

namespace Core.History
{
    public class History<T>
    {
        private List<UpdateInfo<T>> m_updates;
        public IReadOnlyList<UpdateInfo<T>> Updates => m_updates.AsReadOnly();

        public History()
        {
            m_updates = new List<UpdateInfo<T>>();
        }

        public void InitControl(ITrackable<T> trackable)
        {
            var update = new UpdateInfo<T>
            {
                stateAfter = trackable.GetState(),
                timeframe = -1,
                updateCode = UpdateCode.control
            };
            m_updates.Add(update);
        }

        public void Add(ITrackable<T> trackable, UpdateCode updateCode)
        {
            var update = new UpdateInfo<T>
            {
                stateAfter = trackable.GetState(),
                timeframe = trackable.World.GetNextTimeFrame(),
                updateCode = updateCode
            };
            m_updates.Add(update);
        }

        public void Clear()
        {
            if (m_updates.Count == 0)
            {
                return;
            }
            var update = new UpdateInfo<T>
            {
                stateAfter = m_updates.Last().stateAfter,
                timeframe = -1,
                updateCode = UpdateCode.control
            };
            m_updates.Clear();
            // add the control state. this is the initial state at the start of history
            m_updates.Add(update);
        }

        public T GetStateBefore(UpdateCode updateCode)
        {
            // first, find the displacement event
            for (int i = m_updates.Count - 1; i > 0; i--)
            {
                if (m_updates[i].updateCode == updateCode)
                {
                    // return the state before that 
                    return m_updates[i - 1].stateAfter;
                }
            }
            // otherwise, return the last state
            return m_updates.Last().stateAfter;
        }
    }
}