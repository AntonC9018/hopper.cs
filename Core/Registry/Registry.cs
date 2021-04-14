using System;
using Hopper.Shared.Attributes;

namespace Hopper.Core
{
    public struct IdentifierAssigner
    {
        public int offset;

        public int Next()
        {
            return offset++;
        }
    }

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
            return rankNumbers[(int)rank]++;
        }
    }

    public struct Registry
    {
        public static Registry Global;
        
        static Registry()
        {
            Global = new Registry();
            Global.Init();
        }

        public int _currentMod;
        public IdentifierAssigner _component;
        public PriorityAssigner _priority;

        public void Init()
        {
            _priority.Init();
        }

        public int NextMod()
        {
            return ++_currentMod;
        }

        public Identifier NextComponentId()
        {
            return new Identifier(_currentMod, _component.Next());
        }

        public int NextPriority(PriorityRank rank)
        {
            return _priority.Next(rank);
        }
    }
}