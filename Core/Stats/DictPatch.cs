using System.Collections.Generic;
using Hopper.Core.Registries;
using Hopper.Core.Stat.Basic;

namespace Hopper.Core.Stat
{
    public class DictPatchSubRegistry<T> : PatchSubArea<T> where T : SourceBase<T>
    {
        public DictFile CreateFile()
        {
            var file = new DictFile();
            file.content = new Dictionary<int, int>();
            foreach (int id in patches.Keys)
            {
                file.content[id] = patches[id].resistance;
            }
            return file;
        }
    }

}