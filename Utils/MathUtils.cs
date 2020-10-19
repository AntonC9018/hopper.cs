using System;

namespace Utils
{
    public static class Maths
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static int Min(int x, int y)
        {
            return x < y ? x : y;
        }

        internal static int Abs(int x)
        {
            return x < 0 ? -x : x;
        }

        internal static int Max(int x, int y)
        {
            return x > y ? x : y;
        }
    }
}