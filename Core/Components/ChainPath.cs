namespace Hopper.Core.Components
{
    public struct ChainPath<T>
    {
        public System.Func<Entity, T> Chain;
        public T FactoryChain(EntityFactory factory) => Chain(factory.subject);
    }
}