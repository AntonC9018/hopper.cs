using System;
using Hopper.Core.FS;

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
            if (index == deck.Count - 1)
            {
                ReshuffleDeck(rng);
            }
            var item = deck[index];
            index++;
            return item;
        }
    }
}