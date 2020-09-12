using System.Collections.Generic;
using Vector;

namespace Core
{
    public class History
    {
        public class EntityState // possible include some more data
        {
            public IntVector2 pos;
            public IntVector2 orientation;

            public EntityState(Entity entity)
            {
                pos = entity.Pos;
                orientation = entity.Orientation;
            }
        }

        // Don't know what this will be exactly
        // these values are placeholders
        // It may be a good idea to associate one event to each decorator
        public enum EventCode
        {
            attacking_do,
            attacked_do,
            displaced_do,
            move_do,
            pushed_do,
            statused_do,
            Hurt,
            Dead
        }

        public class EventInfo
        {
            public EntityState stateAfter;
            public EventCode eventCode;
        }

        private List<EventInfo>[] m_eventsByPhase;
        public List<EventInfo>[] Phases => m_eventsByPhase;

        public History()
        {
            m_eventsByPhase = new List<EventInfo>[Core.World.s_numPhases];
            for (int i = 0; i < Core.World.s_numPhases; i++)
                m_eventsByPhase[i] = new List<EventInfo>();
        }
        public void Add(Entity entity, EventCode eventCode)
        {
            var state = new EntityState(entity);
            var ev = new EventInfo
            {
                stateAfter = state,
                eventCode = eventCode
            };
            m_eventsByPhase[entity.World.m_state.m_phase].Add(ev);
        }
        public EventInfo Find(System.Predicate<EventInfo> pred)
        {
            foreach (var events in m_eventsByPhase)
            {
                var ev = events.Find(pred);
                if (ev != null)
                    return ev;
            }
            return null;
        }

        public EventInfo FindLast(System.Predicate<EventInfo> pred)
        {
            foreach (var events in m_eventsByPhase)
            {
                var ev = events.FindLast(pred);
                if (ev != null)
                    return ev;
            }
            return null;
        }

        public EventInfo Find(System.Func<EventInfo, int, bool> pred)
        {
            for (int i = 0; i < m_eventsByPhase.Length; i++)
            {
                var events = m_eventsByPhase[i];
                var ev = events.Find(e => pred(e, i));
                if (ev != null)
                    return ev;
            }
            return null;
        }

        public void Clear()
        {
            foreach (var events in m_eventsByPhase)
                events.Clear();
        }
    }
}