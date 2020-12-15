using System.Collections.Generic;
using Hopper.Core.Registry;

namespace Hopper.Core.Stats
{
    public class DictPatchSubRegistry<T> : PatchSubArea<T> where T : IKind
    {
        public DictFile CreateFile()
        {
            var file = new DictFile();
            file.content = new Dictionary<int, int>();
            foreach (int id in patches.Keys)
            {
                file.content[id] = patches[id].Id;
            }
            return file;
        }
    }

}