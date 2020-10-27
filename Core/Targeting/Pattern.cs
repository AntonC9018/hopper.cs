using System.Collections.Generic;
using Core.Utils.Vector;

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
}