using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Hopper.Core;
using Hopper.Core.ActingNS;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Core.Items;
using Hopper.Core.Retouchers;
using Hopper.Core.Stat;
using Hopper.Core.Targeting;
using Hopper.Core.WorldNS;
using Hopper.TestContent;
using Hopper.TestContent.BindingNS;
using Hopper.TestContent.PinningNS;
using Hopper.TestContent.SimpleMobs;
using Hopper.Utils;
using Hopper.Utils.Vector;
using static Hopper.Utils.Vector.IntVector2;

namespace Hopper.Mine
{

    public class Program
    {
        public static IEnumerable<IntVector2> Basic(IntVector2 diff, IntVector2 orientation)
        {
            var diff_ones = diff.Sign();
            var diff_x = new IntVector2(diff_ones.x, 0);
            var diff_y = new IntVector2(0, diff_ones.y);

            // The difference is not diagonal
            if      (diff_ones.x == 0) yield return diff_y;
            else if (diff_ones.y == 0) yield return diff_x;

            // the difference is diagonal
            else
            {
                int dot_x = diff_x.Dot(orientation);
                int dot_y = diff_y.Dot(orientation);

                if (dot_x >= dot_y)
                {
                    yield return diff_x;
                    yield return diff_y;
                } 
                else
                {
                    yield return diff_y;
                    yield return diff_x;
                }
            }
        }

        public static void Main(string[] args)
        {
            foreach (var diff in CircleAroundOrigin)
            if (diff != Zero)
            foreach (var orientation in OrthogonallyAdjacentToOrigin)
            {
                Console.Write($"diff {diff}, orientation {orientation}: ");
                foreach (var dir in Basic(diff, orientation))
                {
                    Console.Write($"{dir}, ");
                }
                Console.WriteLine();
            }
        }
    }
}