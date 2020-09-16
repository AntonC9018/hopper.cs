namespace Core
{
    public class Push : StatFile
    {
        public int source = 0;
        public int power = 1;
        public int distance = 1;
        public int pierce = 1;

        public static Source BasicSource;

        public class Resistance : StatFile
        {
            public int pierce = 0;
        }

        public class Source : IHaveId
        {
            private int m_id;
            public int Id => m_id;

            public Source(int defaultResValue = 1)
            {
                m_id = IdMap.PushSources.Add(this);
                PushSetup.SourceResFile.content.Add(m_id, defaultResValue);
            }
        }
    }
}