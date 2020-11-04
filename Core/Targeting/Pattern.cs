using System.Collections.Generic;
using Core.Utils.Vector;

namespace Core.Targeting
{
    public interface IPattern
    {
        IEnumerable<Piece> GetPieces(IWorldSpot spot, IntVector2 dir);
    }

    public class Pattern : IPattern
    {
        public IEnumerable<Piece> GetPieces(IWorldSpot spot, IntVector2 dir)
        {
            double angle = IntVector2.Right.AngleTo(dir);
            foreach (var piece in pieces)
            {
                yield return piece.Rotate(angle);
            }
        }

        private IList<Piece> pieces;

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