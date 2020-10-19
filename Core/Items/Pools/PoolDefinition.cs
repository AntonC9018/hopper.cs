using System.Collections.Generic;
using Core.FS;

namespace Core.Items
{
    public class PoolDefinition<SP> : FS<Pool> where SP : SubPool
    {
        public Dictionary<int, PoolItem> items = new Dictionary<int, PoolItem>();
        public void RegisterItem(PoolItem item)
        {
            items[item.id] = item;
        }
        public void RegisterItems(IEnumerable<PoolItem> items)
        {
            foreach (var item in items)
            {
                this.items[item.id] = item;
            }
        }
        public void AddItemToPool(PoolItem item, string path)
        {
            var subPool = (SubPool)GetFile(path);
            subPool.items.Add(item.id, item);
        }
        public void AddItemsToPool(IEnumerable<PoolItem> items, string path)
        {
            var subPool = (SubPool)GetFile(path);
            foreach (var item in items)
                subPool.items.Add(item.id, item);
        }
    }
}