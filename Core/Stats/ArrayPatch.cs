using System.Collections.Generic;
using Hopper.Core.Registries;

namespace Hopper.Core.Stats
{
    public class ArrayPatch<T> : IPatchSubArea<T>
    {
        public ArrayFile DefaultFile
        {
            get
            {
                int patchCount = patches.Count;
                var file = new ArrayFile();
                file.content = patches.ToArray();
                return file;
            }
        }
        public List<int> patches = new List<int>();
        public T TryGet(int id) => throw new System.Exception("Unsupported operation");
    }

}