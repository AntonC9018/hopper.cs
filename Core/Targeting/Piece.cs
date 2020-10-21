using System.Collections.Generic;
using Utils.Vector;

namespace Core.Targeting
{
    public class Pattern
    {
        public IList<Piece> pieces;

        public Pattern(params Piece[] pieces)
        {
            this.pieces = pieces;
            for (int i = 0; i < pieces.Length; i++)
            {
                pieces[i].index = i;
            }
        }

        public static Pattern Default = new Pattern
        (
            new Piece
            {
                pos = IntVector2.Right,
                dir = IntVector2.Right,
                reach = null
            }
        );

        public static Pattern Under = new Pattern
        (
            new Piece
            {
                pos = IntVector2.Zero,
                dir = IntVector2.Right,
                reach = null
            }
        );
    }

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
    }
}