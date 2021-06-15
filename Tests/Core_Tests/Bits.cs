using System.Linq;
using Hopper.Core;
using Hopper.Utils;
using NUnit.Framework;
using NAssert = NUnit.Framework.Assert;

namespace Hopper.Tests
{
    public class BitsTests
    {
        public BitsTests()
        {
        }

        [Test]
        public void Stuff()
        {
            NAssert.True(new int[] { 2, 4, 6, 8, 10, 12, 14 }.SequenceEqual(0b_1110.GetBitCombinations()));
            NAssert.True(new int[] { 2, 4, 8 }.SequenceEqual(0b_1110.GetSetBits()));
        }
    }
}