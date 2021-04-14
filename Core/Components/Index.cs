namespace Hopper.Core.Components
{
    public struct Index<T> where T : IComponent
    {
        public Identifier Id;
    }

    public interface IPath{}

    public struct Path<T> : IPath
    {
        public System.Func<Entity, T> Chain;
        public T FactoryChain(EntityFactory factory) => Chain(factory.subject);
    }
}