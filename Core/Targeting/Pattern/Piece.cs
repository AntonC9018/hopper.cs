using Hopper.Utils.Vector;

namespace Hopper.Core.Targeting
{
    public class Piece
    {
        public IntVector2 relativePosition;
        public IntVector2 knockbackDirection;
        public Reach reach;

        public Piece(IntVector2 relativePosition, IntVector2 knockbackDirection, Reach reach)
        {
            this.relativePosition = relativePosition;
            this.knockbackDirection = knockbackDirection;
            this.reach = reach;
        }

        public Piece(IntVector2 relativePosition, IntVector2 knockbackDirection)
        {
            this.relativePosition = relativePosition;
            this.knockbackDirection = knockbackDirection;
            this.reach = new Reach(true);
        }

        public Piece Rotate(double angle)
        {
            return new Piece
            (
                relativePosition : relativePosition.RotateAndRound(angle),
                knockbackDirection : knockbackDirection.RotateAndRound(angle),
                reach : reach
            );
        }

        public Piece Rotate(IntVector2 direction)
        {
            double angle = IntVector2.Right.AngleTo(direction);
            return new Piece
            (
                relativePosition : relativePosition.RotateAndRound(angle),
                knockbackDirection : knockbackDirection.RotateAndRound(angle),
                reach : reach
            );
        }

        public static readonly Piece Default = new Piece
        (
            relativePosition : IntVector2.Right,
            knockbackDirection : IntVector2.Right,
            reach : new Reach(true)
        );

        public static readonly Piece Under = new Piece
        (
            relativePosition : IntVector2.Zero,
            knockbackDirection : IntVector2.Right,
            reach : new Reach(true)
        );
    }
}