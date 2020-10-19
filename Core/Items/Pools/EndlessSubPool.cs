using System.Collections.Generic;

namespace Core.Items
{
    public class EndlessSubPool : SubPool
    {
        public override SubPool Copy(Dictionary<int, PoolItem> _items)
        {
            var sp = new EndlessSubPool();
            foreach (var id in this.items.Keys)
            {
                sp.items.Add(id, _items[id]);
            }
            return sp;
        }

        public override PoolItem GetNextItem()
        {
            if (index == deck.Count - 1)
            {
                ReshuffleDeck();
            }
            var item = deck[index];
            index++;
            return item;
        }
    }
}