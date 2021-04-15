using System.Collections.Generic;
using Hopper.Utils.Vector;

namespace Hopper.Core
{
    public static partial class Movs
    {
        public static IEnumerable<IntVector2> Basic(Transform transform)
        {
            if (transform.TryGetClosestPlayerTransform(out var playerTransform))
            {
                var orientation = transform.orientation;
                var diff = playerTransform.position - transform.position;
                var diff_ones = diff.Sign();
                var diff_x = new IntVector2(diff_ones.x, 0);
                var diff_y = new IntVector2(0, diff_ones.y);

                if (diff_ones.x * orientation.x > 0)
                {
                    yield return diff_x;
                    if (diff_ones.y != 0) { yield return diff_y; }
                }
                else if (diff_ones.y * orientation.y > 0)
                {
                    yield return diff_y;
                    if (diff_ones.x != 0) { yield return diff_x; }
                }
                else
                {
                    if (diff_ones.x != 0) { yield return diff_x; }
                    if (diff_ones.y != 0) { yield return diff_y; }
                }
            }
        }

        public static IEnumerable<IntVector2> Adjacent(Transform transform)
        {
            if (transform.TryGetClosestPlayerTransform(out var playerTransform))
            {
                var difference = playerTransform.position - transform.position;
                var direction  = difference.Sign();

                if (direction.x != 0 && direction.y != 0)
                    yield return direction;
                if (direction.x != 0)
                    yield return new IntVector2(direction.x, 0);
                if (direction.y != 0)
                    yield return new IntVector2(0, direction.y);
            }
        }

        public static IEnumerable<IntVector2> Straight(Transform transform)
        {
            yield return transform.orientation == IntVector2.Zero 
                ? IntVector2.Right
                : transform.orientation;
        }

        public static IEnumerable<IntVector2> Diagonal(Transform transform)
        {
            if (transform.TryGetClosestPlayerTransform(out var playerTransform))
            {
                var diff = (playerTransform.position - transform.position).Sign();
                var orientation = transform.orientation == IntVector2.Zero ? diff : transform.orientation;

                if (diff.x == 0)
                {
                    yield return new IntVector2(orientation.x, diff.y);
                    yield return new IntVector2(-orientation.x, diff.y);
                }
                else if (diff.y == 0)
                {
                    yield return new IntVector2(diff.x, orientation.y);
                    yield return new IntVector2(diff.x, -orientation.y);
                }
                else
                {
                    yield return diff;
                }
            }
        }
    }

}