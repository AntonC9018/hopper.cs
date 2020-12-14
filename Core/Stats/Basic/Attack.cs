namespace Hopper.Core.Stats.Basic
{
    public partial class Attack : StatFile
    {
        public int sourceId;
        public int power;
        public int damage;
        public int pierce;

        public static readonly Source BasicSource = new Source { resistance = 1 };
        public static readonly SimpleStatPath<Attack> Path =
            new SimpleStatPath<Attack>
            (
                "atk/base",
                new Attack
                {
                    sourceId = BasicSource.Id,
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

            public static readonly SimpleStatPath<Resistance> Path = new SimpleStatPath<Resistance>(
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
            public static readonly DictPatchWrapper<Source> Resistance
                = new DictPatchWrapper<Source>("atk/src_res");
        }
    }
}