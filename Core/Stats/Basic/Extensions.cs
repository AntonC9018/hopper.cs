namespace Hopper.Core.Stat.Basic
{
    public static class StatExtensions
    {
        public static Move ToMove(this Push push)
        {
            return new Move
            {
                power = push.distance,
                through = 0
            };
        }
    }
}