namespace Hopper.Core.Stats.Basic
{
    public class Move : StatFile
    {
        public int power;
        public int through;

        public static readonly StatPath<Move> Path = new StatPath<Move>(
            "move/base",
            new Move
            {
                power = 1,
                through = 0
            }
        );
    }
}