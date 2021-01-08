using System.Collections.Generic;
using Hopper.Core.Predictions;
using Hopper.Utils.Vector;

namespace Hopper.Core.Targeting
{
    public interface IPattern : IBehaviorPredictable
    {
        IEnumerable<Piece> GetPieces(IntVector2 dir);
    }

    public class Pattern : IPattern
    {
        public IEnumerable<Piece> GetPieces(IntVector2 dir)
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

        public static Pattern Default = new Pattern(Piece.Default);
        public static Pattern Under = new Pattern(Piece.Under);

        public IEnumerable<IntVector2> Predict(IntVector2 direction)
        {
            foreach (var piece in GetPieces(direction))
            {
                yield return piece.pos;
            }
        }
    }
}