using Hopper.Utils.Chains;

namespace Hopper.Core.Components
{
    public static class PathExtensions
    {
        public static bool TryGet<T>(this IPath<T> path, Entity entity, out T thing)
        {
            thing = path.Get(entity);
            return thing != null;
        }
    }

    public interface IPath<out T>
    {
        /// <summary>
        /// Returns the thing the path points to stored on the entity.
        /// If the entity does not contain the given thing, returns null.
        /// </summary>
        T Get(Entity entity);
    }

    public readonly struct BehaviorChainPath<B, T> : IPath<T> 
        where T : IChain 
        where B : IComponent
    {
        /// <summary>
        /// Returns the chain if the entity has the behavior with which the chain is associated
        /// Otherwise, null is returned.
        /// </summary>
        public readonly System.Func<B, T> Chain;
        public readonly Index<B> BehaviorIndex;

        public BehaviorChainPath(Index<B> index, System.Func<B, T> GetChainFunction)
        {
            BehaviorIndex = index;
            Chain = GetChainFunction;
        }

        public T Get(Entity entity) 
        {
            if (entity.TryGetComponent(BehaviorIndex, out var component))
                return Chain(component);
            return default;
        }

        public override string ToString()
        {
            return $"Path to {BehaviorIndex}";
        }
    }
}