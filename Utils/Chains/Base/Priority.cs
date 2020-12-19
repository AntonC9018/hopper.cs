namespace Hopper.Utils.Chains
{
    public enum PriorityRank
    {
        Lowest = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        Highest = 4,
        Default = Medium
    }

    public static class PriorityMapping
    {
        public static readonly int Lowest = 0x_0001_0000;
        public static readonly int Low = 0x_0010_0000;
        public static readonly int Medium = 0x_0011_0000;
        public static readonly int High = 0x_0100_0000;
        public static readonly int Highest = 0x_0101_0000;
        public static readonly int Default = Medium;
    }
}