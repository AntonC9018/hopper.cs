using Vector;
using System.Collections.Generic;

namespace Core
{
    // This indicates the order in which actions are executed
    public enum Layer
    {
        REAL = 0b_0000_0001,
        MISC = 0b_0000_0010,
        WALL = 0b_0000_0100,
        PROJECTILE = 0b_0000_1000,
        GOLD = 0b_0001_0000,
        FLOOR = 0b_0010_0000,
        TRAP = 0b_0100_0000,
        DROPPED = 0b_1000_0000,
        BLOCK = REAL | WALL | MISC,
    }

    public static class LayerExtensions
    {
        public static int ToIndex(this Layer layer)
        {
            return System.Numerics.BitOperations.Log2((uint)layer);
        }
    }

    public class Cell
    {

        public IntVector2 m_pos;
        public List<Entity> m_entities = new List<Entity>();
        public event System.Action<Entity> EnterEvent;
        public event System.Action<Entity> LeaveEvent;

        public Entity GetFirstEntity()
        {
            return m_entities[0];
        }
        public Entity GetEntityFromLayer(Layer layer)
        {
            foreach (var e in m_entities)
            {
                if ((e.Layer & layer) != 0)
                {
                    return e;
                }
            }
            return null;
        }

        public void FireEnterEvent(Entity entity)
        {
            EnterEvent?.Invoke(entity);
        }

        public void FireLeaveEvent(Entity entity)
        {
            LeaveEvent?.Invoke(entity);
        }
    }
}