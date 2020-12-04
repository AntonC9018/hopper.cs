using System.Collections.Generic;
using Core.Utils.Vector;

namespace Core.Targeting
{
    public class Piece
    {
        public IntVector2 pos;
        public IntVector2 dir;
        // null is no checking required
        // empty list to check all previous indeces
        // list of indices to check the specified indeces
        public List<int> reach;
        public int index;

        public Piece Rotate(double angle)
        {
            return new Piece
            {
                pos = pos.RotateAndRound(angle),
                dir = dir.RotateAndRound(angle),
                reach = reach
            };
        }

        public Piece Rotate(IntVector2 direction)
        {
            double angle = IntVector2.Right.AngleTo(direction);
            return new Piece
            {
                pos = pos.RotateAndRound(angle),
                dir = dir.RotateAndRound(angle),
                reach = reach
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