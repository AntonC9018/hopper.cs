using System.Collections.Generic;
using Hopper.Utils;

namespace Hopper.Core.Components
{
    public class ChainName : ScalableEnum
    {
        private static List<ChainName> chainNames = new List<ChainName>();

        public static readonly ChainName Check = new ChainName("Check");
        public static readonly ChainName Do = new ChainName("Do");
        public static readonly ChainName Success = new ChainName("Success");
        public static readonly ChainName Fail = new ChainName("Fail");
        public static readonly ChainName Condition = new ChainName("Condition");
        public static readonly ChainName Default = new ChainName("Default");

        public ChainName(string name) : base(name, chainNames.Count)
        {
            chainNames.Add(this);
        }

        public static implicit operator ChainName(int value)
        {
            return chainNames[value];
        }
    }
}