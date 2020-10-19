using System;
using System.Collections.Generic;
using Utils;

namespace Core.Items
{
    public class NormalSubPool : SubPool
    {
        public override SubPool Copy(Dictionary<int, PoolItem> _items)
        {
            var sp = new NormalSubPool();
            foreach (var id in this.items.Keys)
            {
                sp.items.Add(id, _items[id]);
            }
            return sp;
        }

        public override PoolItem GetNextItem()
        {
            if (index >= deck.Count)
            {
                return null;
            }
            var item = deck[index];
            index++;
            if (item.q == 0)
                return GetNextItem();
            item.q--;
            return item;
        }
    }
}