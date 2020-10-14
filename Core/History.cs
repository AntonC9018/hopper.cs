using System.Collections.Generic;
using System.Linq;
using Utils.Vector;

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
        // It may be a good idea to associate one update to each decorator
        public enum UpdateCode
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

        public class UpdateInfo
        {
            public EntityState stateAfter;
            public UpdateCode updateCode;
        }

        private List<UpdateInfo>[] m_updatesByPhase;
        public List<UpdateInfo>[] Phases => m_updatesByPhase;

        public History()
        {
            m_updatesByPhase = new List<UpdateInfo>[Core.World.s_numPhases];
            for (int i = 0; i < Core.World.s_numPhases; i++)
                m_updatesByPhase[i] = new List<UpdateInfo>();
        }
        public void Add(Entity entity, UpdateCode updateCode)
        {
            var state = new EntityState(entity);
            var ev = new UpdateInfo
            {
                stateAfter = state,
                updateCode = updateCode
            };
            m_updatesByPhase[entity.World.m_state.m_phase].Add(ev);
        }
        public UpdateInfo Find(System.Predicate<UpdateInfo> pred)
        {
            foreach (var updates in m_updatesByPhase)
            {
                var ev = updates.Find(pred);
                if (ev != null)
                    return ev;
            }
            return null;
        }

        public UpdateInfo FindLast(System.Predicate<UpdateInfo> pred)
        {
            foreach (var updates in m_updatesByPhase.Reverse())
            {
                var ev = updates.FindLast(pred);
                if (ev != null)
                    return ev;
            }
            return null;
        }

        public UpdateInfo Find(System.Func<UpdateInfo, int, bool> pred)
        {
            for (int i = 0; i < m_updatesByPhase.Length; i++)
            {
                var updates = m_updatesByPhase[i];
                var ev = updates.Find(e => pred(e, i));
                if (ev != null)
                    return ev;
            }
            return null;
        }

        public void Clear()
        {
            foreach (var updates in m_updatesByPhase)
                updates.Clear();
        }
    }
}