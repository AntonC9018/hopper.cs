using System.Collections.Generic;
using Hopper.Core.WorldNS;
using Hopper.Utils.Vector;

namespace Hopper.Core.Targeting
{
    public class PieceAttackPattern
    {
        public IEnumerable<AttackTargetContext> MakeContexts(IntVector2 position, IntVector2 direction)
        {
            double angle = IntVector2.Right.AngleTo(direction);

            for (int i = 0; i < pieces.Length; i++)
            {
                var targetPosition = position + pieces[i].relativePosition.RotateAndRound(angle);

                if (World.Global.grid.IsInBounds(targetPosition))
                {
                    yield return new AttackTargetContext(
                        targetPosition,
                        pieces[i].knockbackDirection.RotateAndRound(angle),
                        i    
                    );
                }
            }
        }

        public Piece[] pieces;

        public PieceAttackPattern(params Piece[] pieces)
        {
            this.pieces = pieces;
        }

        public static PieceAttackPattern Default = new PieceAttackPattern(Piece.Default);
        public static PieceAttackPattern Under = new PieceAttackPattern(Piece.Under);
    }
}