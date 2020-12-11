namespace Hopper.Core.Stats.Basic
{
    public partial class Attack : StatFile
    {
        public int sourceId;
        public int power;
        public int damage;
        public int pierce;

        public static Source BasicSource = new Source { resistance = 1 };

        public static StatPath<Attack> Path =
            new StatPath<Attack>
            (
                "atk/base",
                registry => new Attack
                {
                    sourceId = BasicSource.GetId(registry),
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

            public static SimpleStatPath<Resistance> Path = new SimpleStatPath<Resistance>(
                "atk/res",
                new Resistance
                {
                    armor = 0,
                    minDamage = 1,
                    maxDamage = 10,
                    pierce = 1
                }
            );
        }

        public class Source : SourceBase<Source>
        {
            public static class Resistance
            {
                public static StatPath<ArrayFile> Path =
                    new StatPath<ArrayFile>
                    (
                        "atk/src_res",
                        ArrayFilePath<Source>.GetDefaultFile
                    );
            }
        }
    }
}