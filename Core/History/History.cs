using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public void Add(ITrackable<T> trackable, UpdateCode updateCode)
        {
            var state = trackable.GetState();
            var ev = new UpdateInfo<T>
            {
                stateAfter = state,
                timeframe = trackable.World.GetNextTimeFrame(),
                updateCode = updateCode
            };
            m_updates.Add(ev);
        }

        public void Clear()
        {
            m_updates.Clear();
        }
    }
}