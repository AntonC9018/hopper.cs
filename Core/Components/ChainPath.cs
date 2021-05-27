using Hopper.Utils.Chains;

namespace Hopper.Core.Components
{
    public static class PathExtensions
    {
        public static bool TryFollow<T>(this IPath<T> path, Entity entity, out T thing)
        {
            thing = path.Follow(entity);
            return thing != null;
        }
    }

    public interface IPath<out T>
    {
        T Follow(Entity entity);
    }

    public readonly struct BehaviorChainPath<T> : IPath<T> where T : IChain
    {
        /// <summary>
        /// Returns the chain if the entity has the behavior with which the chain is associated
        /// Otherwise, null is returned.
        /// </summary>
        public readonly System.Func<Entity, T> Chain;

        public BehaviorChainPath(System.Func<Entity, T> GetChainFunction)
        {
            Chain = GetChainFunction;
        }

        public T FactoryChain(EntityFactory factory) => Chain(factory.subject);

        T IPath<T>.Follow(Entity entity) => Chain(entity);
    }
}