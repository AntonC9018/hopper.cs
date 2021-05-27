using Hopper.Core.Items;

namespace Hopper.Core
{
    public struct PoolRegistry
    {
        public IdentifierAssigner _assigner;
        public Pool pool;

        public void Init()
        {
            pool = new Pool();
        }

        public void StartRuntime()
        {
            pool = new Pool(pool);
        }

        public void StopRuntime()
        {
            pool = pool.templatePool;
        }

        public Identifier RegisterSubPool(SubPool subpool)
        {
            var id = new Identifier(Registry.Global._currentMod, _assigner.Next());
            pool.Add(id, subpool);
            return id;
        }

        public EntityFactory Draw(Identifier poolId)
        {
            var entityId = pool.DrawFrom(poolId);
            return Registry.Global._entityFactory.Get(entityId);
        }
    }
}