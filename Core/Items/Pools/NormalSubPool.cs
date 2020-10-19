using System;
using Core.FS;

namespace Core.Items
{
    public class NormalSubPool : SubPool
    {
        public override File Copy()
        {
            var sp = new NormalSubPool();
            foreach (var item in this.items)
            {
                sp.items.Add(item);
            }
            return sp;
        }

        public override PoolItem GetNextItem(Random rng)
        {
            if (index >= deck.Count)
            {
                return null;
            }
            var item = deck[index];
            index++;
            if (item.quantity == 0)
                return GetNextItem(rng);
            item.quantity--;
            return item;
        }
    }
}