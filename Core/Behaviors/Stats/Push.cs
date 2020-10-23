namespace Core.Stats.Basic
{
    public class Push : StatFile
    {
        public int sourceId;
        public int power;
        public int distance;
        public int pierce;

        public static Source BasicSource = new Source();
        public static readonly StatPath<Push> Path = new StatPath<Push>(
            "push/base",
            new Push
            {
                sourceId = BasicSource.Id,
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
                Resistance.Path.DefaultFile.Add(m_id, defaultResValue);
            }

            public class Resistance : MapFile
            {
                public static readonly StatPath<Resistance> Path = new StatPath<Resistance>("push/src_res");
                protected override MapFile DefaultFile => Path.DefaultFile;
            }
        }
    }
}