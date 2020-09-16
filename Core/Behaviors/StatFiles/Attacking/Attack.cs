using Core.FS;

namespace Core
{
    public class Attack : StatFile
    {
        public int sourceId;
        public int power;
        public int damage;
        public int pierce;

        public static Source BasicSource;


        public class Resistance : StatFile
        {
            public int armor;
            public int minDamage;
            public int maxDamage;
            public int pierce;
        }

        public class Source : IHaveId
        {
            private int m_id;
            public int Id => m_id;

            public Source(int defaultResValue = 1)
            {
                m_id = IdMap.AttackSources.Add(this);
                AttackSetup.SourceResFile.content.Add(m_id, defaultResValue);
            }
        }
    }
}