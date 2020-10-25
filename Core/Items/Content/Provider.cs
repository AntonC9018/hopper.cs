// this will need to be refactored to include 
// 1. amounts
// 2. be polymorphic
namespace Core.Items
{
    public enum ContentType
    {
        GOLD, ENTITY, ITEM
    }

    public class ContentConfig
    {
        public ContentType type;
        public bool isAforeset;

        // gold
        public int amount;

        // enemy
        public IFactory<Entity> factory;

        // item
        public IItem item;

        public string poolPath;
    }

    public interface IProvider
    {
        IContent CreateContent(ContentConfig config);
    }

    public class ContentProvider : IProvider
    {
        public static ContentProvider DefaultProvider = new ContentProvider();

        public ISuperPool entityPool;
        public ISuperPool itemPool;

        // TODO: polymorphic content. only the pools are selected based on types (probably)
        public IContent CreateContent(ContentConfig config)
        {
            if (entityPool == null || itemPool == null)
            {
                throw new System.Exception("Set up the pools prior to using the content provider.");
            }

            if (config.type == ContentType.ENTITY)
            {
                if (config.isAforeset)
                {
                    return new EntityContent(config.factory);
                }

                var poolItem = entityPool.GetNextItem(config.poolPath);
                if (poolItem == null)
                {
                    return null;
                }
                var factory = IdMap.EntityFactory.Map(poolItem.id);
                return new EntityContent(factory);
            }

            else if (config.type == ContentType.ITEM)
            {
                if (config.isAforeset)
                {
                    return new ItemContent(config.item);
                }

                var poolItem = itemPool.GetNextItem(config.poolPath);
                if (poolItem == null)
                {
                    return null;
                }
                var factory = IdMap.Items.Map(poolItem.id);
                return new ItemContent(factory);
            }

            return null;
        }

        public void UseThrowawayPools()
        {
            entityPool = new ThrowawayPool();
            itemPool = new ThrowawayPool();
        }

        public void UsePools(ISuperPool entityPool, ISuperPool itemPool)
        {
            this.entityPool = entityPool;
            this.itemPool = itemPool;
        }
    }
}