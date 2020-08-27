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
    }

}