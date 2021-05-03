namespace Hopper.Core.Components
{
    public readonly struct ChainPath<T>
    {
        /// <summary>
        /// Returns the chain if the entity has the behavior with which the chain is associated
        /// Otherwise, null is returned.
        /// </summary>
        public readonly System.Func<Entity, T> Chain;

        public ChainPath(System.Func<Entity, T> GetChainFunction)
        {
            Chain = GetChainFunction;
        }

        public bool TryGetChain(Entity entity, out T chain)
        {
            chain = Chain(entity);
            return chain != null;
        }

        public T FactoryChain(EntityFactory factory) => Chain(factory.subject);
    }
}