using System.Collections.Generic;

namespace Hopper.Core.Registries
{
    public class PatchSubArea<T> : IPatchSubArea<T>
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