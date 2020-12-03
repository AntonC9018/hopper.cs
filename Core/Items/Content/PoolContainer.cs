namespace Core.Items
{
    public class PoolContainer
    {
        public ISuperPool EntityPool;
        public ISuperPool ItemPool;

        public EntityContent GetEntity(string path)
        {
            var poolItem = EntityPool.GetNextItem(path);
            if (poolItem == null)
            {
                return null;
            }
            var factory = Registry.Default.EntityFactory.Map(poolItem.id);
            return new EntityContent(factory);
        }

        public ItemContent GetItem(string path)
        {
            var poolItem = ItemPool.GetNextItem(path);
            if (poolItem == null)
            {
                return null;
            }
            var item = Registry.Default.Items.Map(poolItem.id);
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