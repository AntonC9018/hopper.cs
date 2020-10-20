namespace Core.Stats.Basic
{
    public class Push : StatFile
    {
        public int source;
        public int power;
        public int distance;
        public int pierce;

        public static Source BasicSource;
        public static readonly StatPath<Push> Path = new StatPath<Push>(
            "push/base",
            new Push
            {
                source = 0,
                power = 1,
                distance = 1,
                pierce = 1
            }
        );

        public class Resistance : StatFile
        {
            public int pierce;
            public static readonly StatPath<Resistance> Path = new StatPath<Resistance>("push/res");
        }

        public class Source : IHaveId
        {
            private int m_id;
            public int Id => m_id;

            public Source(int defaultResValue = 1)
            {
                m_id = IdMap.PushSources.Add(this);
                Resistance.Path.DefaultFile.content.Add(m_id, defaultResValue);
            }

            public class Resistance : MapFile
            {
                public static readonly StatPath<Resistance> Path = new StatPath<Resistance>("push/src_res");
            }
        }
    }
}