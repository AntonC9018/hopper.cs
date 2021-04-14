using System;
using Hopper.Shared.Attributes;

namespace Hopper.Core
{
    public struct PriorityAssigner
    {
        public static readonly int numRanks = Enum.GetValues(typeof(PriorityRank)).Length;
        public int[] rankNumbers;

        public void Init()
        {
            rankNumbers = new int[numRanks];
            var share = int.MaxValue / numRanks;
            for (int i = 1; i < numRanks; i++)
            {
                rankNumbers[i] = rankNumbers[i - 1] + share;
            }
        }

        public int Next(PriorityRank rank)
        {
            return ++rankNumbers[(int)rank];
        }
    }
}