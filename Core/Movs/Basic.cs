using System.Collections.Generic;
using Vector;

namespace Core
{
    public static partial class Movs
    {
        public static List<IntVector2> Basic(Entity e, Action a)
        {
            Entity player = e.GetClosestPlayer();
            var list = new List<IntVector2>();
            if (player == null)
            {
                return list;
            }
            void Add(int x, int y) => list.Add(new IntVector2(x, y));
            var rel = CalculateRelativeness(e, player);

            if (e.m_orientation.x > 0)
            {
                if (rel.gx) Add(1, 0);
                if (rel.gy) Add(0, 1);
                if (rel.lx) Add(0, -1);
                if (rel.ly) Add(-1, 0);
            }
            else if (e.m_orientation.x < 0)
            {
                if (rel.lx) Add(-1, 0);
                if (rel.gy) Add(0, 1);
                if (rel.ly) Add(0, -1);
                if (rel.gx) Add(1, 0);
            }
            else if (e.m_orientation.y > 0)
            {
                if (rel.gy) Add(0, 1);
                if (rel.gx) Add(1, 0);
                if (rel.lx) Add(-1, 0);
                if (rel.ly) Add(0, -1);
            }
            else if (e.m_orientation.y < 0)
            {
                if (rel.ly) Add(0, -1);
                if (rel.gx) Add(1, 0);
                if (rel.lx) Add(-1, 0);
                if (rel.gy) Add(0, 1);
            }
            else
            {
                if (rel.gx) Add(1, 0);
                if (rel.lx) Add(-1, 0);
                if (rel.gy) Add(0, 1);
                if (rel.ly) Add(0, -1);
            }

            return list;
        }

        public static List<IntVector2> Adjacent(Entity e, Action a)
        {
            Entity player = e.GetClosestPlayer();
            var list = new List<IntVector2>();
            if (player == null)
            {
                return list;
            }
            void Add(int x, int y) => list.Add(new IntVector2(x, y));
            var rel = CalculateRelativeness(e, player);

            if (rel.gx)
            {
                if (rel.gy) { Add(1, 1); Add(0, 1); }
                if (rel.ly) { Add(1, -1); Add(0, -1); }
                Add(1, 0);
            }
            else if (rel.lx)
            {
                if (rel.gy) { Add(-1, 1); Add(0, 1); }
                if (rel.ly) { Add(-1, -1); Add(0, -1); }
                Add(-1, 0);
            }
            else
            {
                Add(0, rel.gy ? 1 : -1);
            }

            return list;
        }

        public static List<IntVector2> Straight(Entity e, Action a)
        {
            return new List<IntVector2> { e.m_orientation };
        }

        public static List<IntVector2> Diagonal(Entity e, Action a)
        {
            Entity player = e.GetClosestPlayer();
            var list = new List<IntVector2>();
            if (player == null)
            {
                return list;
            }
            void Add(int x, int y) => list.Add(new IntVector2(x, y));
            var diff = (player.m_pos - e.m_pos).Sign();
            if (diff.x == 0)
            {
                Add(e.m_orientation.x, diff.y);
                Add(-e.m_orientation.x, diff.y);
            }
            else if (diff.y == 0)
            {
                Add(diff.x, e.m_orientation.y);
                Add(diff.x, -e.m_orientation.y);
            }
            else
            {
                list.Add(diff);
            }
            return list;
        }
    }

}