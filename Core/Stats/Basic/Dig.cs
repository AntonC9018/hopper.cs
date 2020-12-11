namespace Hopper.Core.Stats.Basic
{
    public class Dig : StatFile
    {
        public int power;
        public int damage;
        public int pierce;

        public static Attack.Source Source = new Attack.Source { resistance = 1 };

        public static readonly SimpleStatPath<Dig> Path = new SimpleStatPath<Dig>(
            "dig/base",
            new Dig
            {
                power = 0,
                damage = 1,
                pierce = 10
            }
        );

        public Attack ToAttack(Registry registry)
        {
            return new Attack
            {
                sourceId = Source.GetId(registry),
                power = power,
                damage = damage,
                pierce = pierce
            };
        }
    }
}