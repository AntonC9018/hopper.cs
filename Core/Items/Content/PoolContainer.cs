namespace Hopper.Core.Items
{
    public class PoolContainer
    {
        public ISuperPool EntityPool;
        public ISuperPool ItemPool;

        public EntityContent GetEntity(string path, Registry registry)
        {
            var poolItem = EntityPool.GetNextItem(path);
            if (poolItem == null)
            {
                return null;
            }
            var factory = registry.EntityFactory.Get(poolItem.id);
            return new EntityContent(factory);
        }

        public ItemContent GetItem(string path, Registry registry)
        {
            var poolItem = ItemPool.GetNextItem(path);
            if (poolItem == null)
            {
                return null;
            }
            var item = registry.Items.Get(poolItem.id);
            return new ItemContent(item);
        }

        public void UseThrowawayPools()
        {
            EntityPool = new ThrowawayPool();
            ItemPool = new ThrowawayPool();
        }

        public void UsePools(ISuperPool entityPool, ISuperPool itemPool)
        {
            this.EntityPool = entityPool;
            this.ItemPool = itemPool;
        }
    }
}