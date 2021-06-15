using System.Collections.Generic;

namespace Hopper.Utils
{
    public struct AnonymousInt32Flags
    {
        public System.Int32 _value;

        public bool Get(int position)
        {
            return (_value & (1 << position)) != 0;
        }

        public void Set(int position)
        {
            _value |= (1 << position);
        }

        public void Unset(int position)
        {
            _value &= ~(1 << position);
        }
        
        public void Set(int position, bool set)
        {
            if (set) Set(position);
            else     Unset(position);
        }
    }

    public static class BitShenanigans
    {
        public static IEnumerable<int> GetSetBits(this int bits)
        {
            int current = (~bits + 1) & bits;

            while (current != 0)
            {
                yield return current;
                current = (~bits + (current << 1)) & bits;
            }
        }

        public static IEnumerable<int> GetBitCombinations(this int bits)
        {
            int current = 0;
            
            while (true)
            {
                current = (current - bits) & bits;
                if (current == 0) yield break;
                yield return current;
            }
        }
    }
}