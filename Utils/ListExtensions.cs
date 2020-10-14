using System;
using System.Collections.Generic;

namespace Utils
{
    public static class ListExtension
    {
        public static List<T> Where<T>(this List<T> list, System.Func<T, bool> pred)
        {
            var l = new List<T>();
            for (int i = 0; i < list.Count; i++)
            {
                if (pred(list[i]))
                {
                    l.Add(list[i]);
                }
            }
            return l;
        }
        public static List<T> FilterFromIndex<T>(this List<T> list, System.Func<T, bool> pred, int includeAllBefore = 0)
        {
            var l = new List<T>();
            for (int i = 0; i < includeAllBefore; i++)
            {
                l.Add(list[i]);
            }
            for (int i = includeAllBefore; i < list.Count; i++)
            {
                if (pred(list[i]))
                {
                    l.Add(list[i]);
                }
            }
            return l;
        }
        public static void Shuffle<T>(this IList<T> list, Random rng)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static List<T> Copy<T>(this IList<T> list)
        {
            var result = new List<T>(list.Count);
            result.AddRange(list);
            return result;
        }
    }
}