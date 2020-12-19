using Hopper.Core.Registries;

namespace Hopper.Core.Items
{
    public class PoolContainer<T> where T : IKind
    {
        public ISuperPool pool;

        public T Get(string path, Registry registry)
        {
            var poolItem = pool.GetNextItem(path);
            if (poolItem == null)
            {
                return default(T);
            }
            return registry.GetKindRegistry<T>().Get(poolItem.id);
        }
    }

    public class Pools
    {
        public Registry registry;
        public readonly PoolContainer<IFactory<Entity>> entityContainer;
        public readonly PoolContainer<IItem> itemContainer;

        public Pools()
        {
            this.entityContainer = new PoolContainer<IFactory<Entity>>();
            this.itemContainer = new PoolContainer<IItem>();
        }

        public EntityContent GetEntity(string path)
        {
            return new EntityContent(entityContainer.Get(path, registry));
        }

        public ItemContent GetItem(string path)
        {
            return new ItemContent(itemContainer.Get(path, registry));
        }

        public void UseThrowawayPools()
        {
            entityContainer.pool = new ThrowawayPool();
            itemContainer.pool = new ThrowawayPool();
        }

        public void UsePools(ISuperPool entityPool, ISuperPool itemPool)
        {
            entityContainer.pool = entityPool;
            itemContainer.pool = itemPool;
        }
    }
}