using System.Collections.Generic;

namespace Hopper.Core.Stats
{
    public class ArrayPatch<T> : IPatchRegistry<T>
    {
        public ArrayFile DefaultFile
        {
            get
            {
                int patchCount = patches.Count;
                var file = new ArrayFile();
                DefaultFile.content = patches.ToArray();
                return file;
            }
        }
        public List<int> patches = new List<int>();
        public T TryGet(int id) => throw new System.Exception("Unsupported operation");
    }

}