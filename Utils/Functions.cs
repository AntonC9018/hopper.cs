using System;
using System.Collections.Generic;

namespace Hopper.Utils
{
    public static class Functions
    {
        public static Queue<int> ShuffledRangeQueue(int start, int end, Random rng)
        {
            List<int> nums = new List<int>(end - start + 1);
            for (int i = start; i <= end; i++)
            {
                nums.Add(i);
            }
            nums.Shuffle(rng);
            var queue = new Queue<int>();
            for (int i = 0; i < nums.Count; i++)
            {
                queue.Enqueue(nums[i]);
            }
            return queue;
        }
    }
}