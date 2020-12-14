using System.Collections.Generic;

namespace Hopper.Core
{
    // The motivation for this new type of registry is that
    // On initialization entity facctories require some additional data, dependent on the current registry
    // Since I don't want to touch the global state so that the app stays more manageable,
    // I thought, that for at initializtion I would pass the factories the regitry to be used
    // for getting data 

    public interface IPatchSubRegistry<out T>
    {
        T TryGet(int id);
    }

    public class PatchSubRegistry<T> : IPatchSubRegistry<T>
    {
        public Dictionary<int, T> patches = new Dictionary<int, T>();
        public T TryGet(int id)
        {
            if (patches.ContainsKey(id))
                return patches[id];
            return default(T);
        }
        public void Add(int id, T item)
        {
            patches[id] = item;
        }
    }
}