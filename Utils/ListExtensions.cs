using System;
using System.Collections.Generic;

namespace Hopper.Utils
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

        public static List<T> FilterFromIndex<T>(
            this List<T> list, System.Func<T, bool> pred, int includeAllBefore)
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

        public static T Find<T>(this IReadOnlyList<T> list, System.Predicate<T> pred)
        {
            foreach (var el in list)
            {
                if (pred(el))
                    return el;
            }
            return default(T);
        }

        public static T FindLast<T>(this IReadOnlyList<T> list, System.Predicate<T> pred)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (pred(list[i]))
                    return list[i];
            }
            return default(T);
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

        public static T[] Concat<T>(this T[] array, params T[][] arrays)
        {
            int length = array.Length;
            foreach (var arr in arrays)
            {
                length += arr.Length;
            }
            var result = new T[length];
            length = array.Length;
            array.CopyTo(result, 0);
            foreach (var arr in arrays)
            {
                array.CopyTo(arr, length);
                length += arr.Length;
            }
            return result;
        }

        public static IEnumerable<T> ToIEnumerable<T>(this IEnumerator<T> enumerator)
        {
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }

        public static void AddRange<T>(this HashSet<T> set, IEnumerable<T> enumerable)
        {
            foreach (var it in enumerable)
            {
                set.Add(it);
            }
        }

        public static List<U> ConvertAllToList<T, U>(this IEnumerable<T> arr, System.Func<T, U> converter)
        {
            var result = new List<U>();
            foreach (var el in arr)
            {
                result.Add(converter(el));
            }
            return result;
        }

        public static IList<T> Fill<T>(this IList<T> arr, T value)
        {
            for (int i = 0; i < arr.Count; i++)
            {
                arr[i] = value;
            }
            return arr;
        }

        public static bool None<T>(this IEnumerable<T> arr, System.Predicate<T> pred)
        {
            foreach (var value in arr)
            {
                if (pred(value) == true)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool All<T>(this IEnumerable<T> arr, T value)
        {
            foreach (var el in arr)
            {
                if (!el.Equals(value))
                {
                    return false;
                }
            }
            return true;
        }
    }
}