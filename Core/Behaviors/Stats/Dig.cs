namespace Core.Stats.Basic
{
    public class Dig : StatFile
    {
        public int power;
        public int damage;
        public int pierce;

        public static Attack.Source Source = new Attack.Source();
        public static readonly IStatPath<Dig> Path = new StatPath<Dig>(
            "dig/base",
            new Dig
            {
                power = 0,
                damage = 1,
                pierce = 10
            }
        );

        public Attack ToAttack()
        {
            return new Attack
            {
                sourceId = Source.Id,
                power = power,
                damage = damage,
                pierce = pierce
            };
        }
    }
}