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
}