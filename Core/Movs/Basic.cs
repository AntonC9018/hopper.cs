using System.Collections.Generic;
using Core.Utils.Vector;

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

            var orientation = e.Orientation;
            var diff = player.Pos - e.Pos;
            var diff_ones = diff.Sign();
            var diff_x = new IntVector2(diff_ones.x, 0);
            var diff_y = new IntVector2(0, diff_ones.y);

            if (diff_ones.x * orientation.x > 0)
            {
                list.Add(diff_x);
                if (diff_ones.y != 0) { list.Add(diff_y); }
            }
            else if (diff_ones.y * orientation.y > 0)
            {
                list.Add(diff_y);
                if (diff_ones.x != 0) { list.Add(diff_x); }
            }
            else
            {
                if (diff_ones.x != 0) { list.Add(diff_x); }
                if (diff_ones.y != 0) { list.Add(diff_y); }
            }

            // var rel = CalculateRelativeness(e, player);

            // if (e.Orientation.x > 0)
            // {
            //     if (rel.gx) Add(1, 0);
            //     if (rel.gy) Add(0, 1);
            //     if (rel.lx) Add(0, -1);
            //     if (rel.ly) Add(-1, 0);
            // }
            // else if (e.Orientation.x < 0)
            // {
            //     if (rel.lx) Add(-1, 0);
            //     if (rel.gy) Add(0, 1);
            //     if (rel.ly) Add(0, -1);
            //     if (rel.gx) Add(1, 0);
            // }
            // else if (e.Orientation.y > 0)
            // {
            //     if (rel.gy) Add(0, 1);
            //     if (rel.gx) Add(1, 0);
            //     if (rel.lx) Add(-1, 0);
            //     if (rel.ly) Add(0, -1);
            // }
            // else if (e.Orientation.y < 0)
            // {
            //     if (rel.ly) Add(0, -1);
            //     if (rel.gx) Add(1, 0);
            //     if (rel.lx) Add(-1, 0);
            //     if (rel.gy) Add(0, 1);
            // }
            // else
            // {
            //     if (rel.gx) Add(1, 0);
            //     if (rel.lx) Add(-1, 0);
            //     if (rel.gy) Add(0, 1);
            //     if (rel.ly) Add(0, -1);
            // }

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
            return new List<IntVector2> { e.Orientation };
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
            var diff = (player.Pos - e.Pos).Sign();
            if (diff.x == 0)
            {
                Add(e.Orientation.x, diff.y);
                Add(-e.Orientation.x, diff.y);
            }
            else if (diff.y == 0)
            {
                Add(diff.x, e.Orientation.y);
                Add(diff.x, -e.Orientation.y);
            }
            else
            {
                list.Add(diff);
            }
            return list;
        }
    }

}