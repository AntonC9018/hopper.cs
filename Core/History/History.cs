using System.Collections.Generic;
using Hopper.Utils;
using System.Linq;

namespace Hopper.Core.History
{
    public class History<T>
    {
        private List<UpdateInfo<T>> m_updates;
        public IReadOnlyList<UpdateInfo<T>> Updates => m_updates.AsReadOnly();

        public History()
        {
            m_updates = new List<UpdateInfo<T>>();
        }

        public void InitControlUpdate(ITrackable<T> trackable)
        {
            var initialUpdate = CreateControlUpdate(trackable.GetState());
            m_updates.Add(initialUpdate);
        }

        private UpdateInfo<T> CreateControlUpdate(T state)
        {
            return new UpdateInfo<T>
            {
                stateAfter = state,
                timeframe = -1,
                updateCode = UpdateCode.control
            };
        }

        public void Add(T state, int timeframe, UpdateCode updateCode)
        {
            var update = new UpdateInfo<T>
            {
                stateAfter = state,
                timeframe = timeframe,
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
            var controlState = m_updates.Last().stateAfter;
            m_updates.Clear();
            // add the control update. this has the initial state at the start of history
            m_updates.Add(CreateControlUpdate(controlState));
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
            return m_updates[m_updates.Count - 1].stateAfter;
        }
    }

    public static class HistoryExtension
    {
        static public void Add(this History<EntityState> history, Entity entity, UpdateCode updateCode)
        {
            history.Add(
                state: ((ITrackable<EntityState>)entity).GetState(),
                timeframe: entity.World.GetNextTimeFrame(),
                updateCode
            );
        }
    }
}