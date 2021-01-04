using System;
using Hopper.Utils.FS;

namespace Hopper.Core.Items
{
    public class EndlessSubPool : SubPool
    {
        public override File Copy()
        {
            var sp = new EndlessSubPool();
            foreach (var item in this.items)
            {
                sp.items.Add(item);
            }
            return sp;
        }

        public override PoolItem GetNextItem(Random rng)
        {
            if (index == deck.Length)
            {
                ReshuffleDeck(rng);
            }
            return deck[index++];
        }
    }
}