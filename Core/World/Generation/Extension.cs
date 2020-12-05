using System;
using System.Collections.Generic;
using System.Linq;

namespace Hopper.Core.Generation
{
    public static class RandomExtensions
    {
        public static T GetRandom<T>(this IEnumerable<T> enumerable, Random rng)
        {
            return enumerable.ElementAt(rng.Next(enumerable.Count()));
        }
    }
}