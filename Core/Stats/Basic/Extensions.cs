namespace Hopper.Core.Stat
{
    public static class StatExtensions
    {
        public static void ToMove(this Push push, out Move move)
        {
            move = new Move(push.distance, 0);
        }

        public static void ToAttack(this Dig dig, out Attack attack)
        {
            attack = new Attack(dig.damage, dig.pierce, dig.power, Dig.Source.Index);
        }
    }
}