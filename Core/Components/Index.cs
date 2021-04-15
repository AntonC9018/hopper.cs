namespace Hopper.Core.Components
{
    public struct Index<T>
    {
        public Identifier Id;
    }

    public interface IPath{}

    public struct ChainPath<T> : IPath
    {
        public System.Func<Entity, T> Chain;
        public T FactoryChain(EntityFactory factory) => Chain(factory.subject);
    }
}