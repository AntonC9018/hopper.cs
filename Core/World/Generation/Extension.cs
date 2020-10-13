using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Generation
{
    public static class RandomExtensions
    {
        private static readonly Random rnd = new Random();
        private static readonly object sync = new object();

        public static T GetRandom<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.ElementAt(rnd.Next(enumerable.Count()));
        }
    }
}