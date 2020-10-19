namespace Core.Stats.Basic
{
    public class Attack : StatFile
    {
        public int sourceId;
        public int power;
        public int damage;
        public int pierce;

        public static Source BasicSource;
        public static readonly IStatPath<Attack> Path = new StatPath<Attack>(
            "atk/base",
            new Attack
            {
                sourceId = 0,
                power = 1,
                damage = 1,
                pierce = 1
            }
        );


        public class Resistance : StatFile
        {
            public int armor;
            public int minDamage;
            public int maxDamage;
            public int pierce;

            public static readonly IStatPath<Resistance> Path = new StatPath<Resistance>(
                "atk/res",
                new Resistance
                {
                    armor = 0,
                    minDamage = 1,
                    maxDamage = 10,
                    pierce = 1
                });
        }

        public class Source : IHaveId
        {
            private int m_id;
            public int Id => m_id;

            public Source(int defaultResValue = 1)
            {
                m_id = IdMap.AttackSources.Add(this);
                Resistance.Path.DefaultFile.content.Add(m_id, defaultResValue);
            }

            public class Resistance : MapFile
            {
                public static readonly IStatPath<Resistance> Path = new StatPath<Resistance>("atk/src_res");
            }
        }
    }
}