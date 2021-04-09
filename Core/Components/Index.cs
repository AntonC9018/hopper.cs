namespace Hopper.Core.Components
{
    public struct Identifier 
    { 
        int number;
    }

    public struct Index<T> where T : IComponent
    {
        public Identifier componentId;
    }

    public interface IPath{}

    public struct Path<T> : IPath
    {
        public System.Func<Entity, T> Chain;
        public System.Func<EntityFactory, T> FactoryChain;
    }
}