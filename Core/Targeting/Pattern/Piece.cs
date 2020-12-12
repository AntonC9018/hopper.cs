using System.Collections.Generic;
using Hopper.Utils.Vector;

namespace Hopper.Core.Targeting
{
    public class Piece
    {
        // vector offset
        public IntVector2 pos;
        // = knockback direction
        public IntVector2 dir;
        // null is no checking required
        // empty list to check all previous indices
        // list of indices to check the specified indices
        public int[] reach;

        // set automatically, if created for a pattern. otherwise, leave at default
        public int index;

        public Piece Rotate(double angle)
        {
            return new Piece
            {
                pos = pos.RotateAndRound(angle),
                dir = dir.RotateAndRound(angle),
                reach = reach,
                index = index
            };
        }

        public Piece Rotate(IntVector2 direction)
        {
            double angle = IntVector2.Right.AngleTo(direction);
            return new Piece
            {
                pos = pos.RotateAndRound(angle),
                dir = dir.RotateAndRound(angle),
                reach = reach,
                index = index
            };
        }

        public static readonly Piece Default = new Piece
        {
            pos = IntVector2.Right,
            dir = IntVector2.Right,
            reach = null
        };

        public static readonly Piece Under = new Piece
        {
            pos = IntVector2.Zero,
            dir = IntVector2.Right,
            reach = null
        };
    }
}