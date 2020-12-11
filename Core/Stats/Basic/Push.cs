namespace Hopper.Core.Stats.Basic
{
    public class Push : StatFile
    {
        public int sourceId;
        public int power;
        public int distance;
        public int pierce;

        public Move ConvertToMove()
        {
            return new Move
            {
                power = distance,
                through = 0
            };
        }

        public static Source BasicSource = new Source { resistance = 1 };

        public static StatPath<Push> Path =
            new StatPath<Push>
            (
                "push/base",
                registry => new Push
                {
                    sourceId = BasicSource.GetId(registry),
                    power = 1,
                    distance = 1,
                    pierce = 1
                }
            );

        public class Resistance : StatFile
        {
            public int pierce;
            public static readonly SimpleStatPath<Resistance> Path = new SimpleStatPath<Resistance>("push/res");
        }

        public class Source : SourceBase<Source>
        {
            public static class Resistance
            {
                public static StatPath<ArrayFile> Path =
                    new StatPath<ArrayFile>
                    (
                        "push/src_res",
                        ArrayFilePath<Source>.GetDefaultFile
                    );
            }
        }
    }
}