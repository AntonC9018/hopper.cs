using System.Numerics;
using System.Collections.Generic;

namespace Core
{
    // This indicates the order in which actions are executed
    public enum Layer
    {
        REAL = 0,
        MISC = 1,
        WALL = 2,
        PROJECTILE = 3,
        GOLD = 4,
        FLOOR = 5,
        TRAP = 6,
        DROPPED = 7,
    }

    public class CellEvent
    {
        public Entity entity;
    }

    public class Cell
    {

        public Vector2 pos;
        public List<Entity> m_entities = new List<Entity>();
        public event System.EventHandler<CellEvent> EnterEvent;
        public event System.EventHandler<CellEvent> LeaveEvent;

        public Entity GetFirstEntity()
        {
            return m_entities[0];
        }
        public Entity GetEntityFromLayer(Layer layer)
        {
            foreach (var e in m_entities)
            {
                if (e.m_layer == layer)
                {
                    return e;
                }
            }
            return null;
        }

        public void FireEnterEvent(Entity entity)
        {
            System.EventHandler<CellEvent> raiseEv = EnterEvent;
            var ev = new CellEvent { entity = entity };
            if (raiseEv != null)
            {
                raiseEv(this, ev);
            }
        }

        public void FireLeaveEvent(Entity entity)
        {
            System.EventHandler<CellEvent> raiseEv = LeaveEvent;
            var ev = new CellEvent { entity = entity };
            if (raiseEv != null)
            {
                raiseEv(this, ev);
            }
        }
    }
}