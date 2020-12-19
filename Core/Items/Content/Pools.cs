using System;
using Hopper.Core.Registries;

namespace Hopper.Core.Items
{
    public class Pools
    {
        public readonly Registry registry;
        public readonly PoolContainer<IFactory<Entity>> entityContainer;
        public readonly PoolContainer<IItem> itemContainer;

        public Pools(Registry registry)
        {
            this.entityContainer = new PoolContainer<IFactory<Entity>>();
            this.itemContainer = new PoolContainer<IItem>();
            this.registry = registry;
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

        public Pools Copy()
        {
            var pools = new Pools(registry);
            pools.entityContainer.pool = this.entityContainer.pool.Copy();
            pools.itemContainer.pool = this.itemContainer.pool.Copy();
            return pools;
        }
    }
}