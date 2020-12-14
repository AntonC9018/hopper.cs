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

        public static readonly Source BasicSource = new Source { resistance = 1 };
        public static readonly SimpleStatPath<Push> Path =
            new SimpleStatPath<Push>
            (
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
            public static readonly SimpleStatPath<Resistance> Path
                = new SimpleStatPath<Resistance>("push/res");
        }

        public class Source : SourceBase<Source>
        {
            public static readonly DictPatchWrapper<Source> Resistance
                = new DictPatchWrapper<Source>("push/src_res");
        }
    }
}