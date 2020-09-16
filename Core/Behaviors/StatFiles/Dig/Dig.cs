namespace Core
{
    public class Dig : StatFile
    {
        public int power;
        public int damage;
        public int pierce;

        public static Attack.Source Source;

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