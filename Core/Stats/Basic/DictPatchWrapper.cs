using Hopper.Core.Registry;
using Hopper.Core.Stats.Basic;

namespace Hopper.Core.Stats
{
    public class DictPatchWrapper<T> where T : SourceBase<T>
    {
        public readonly DictStatPath Path;

        public DictPatchWrapper(string path)
        {
            Path = new DictStatPath(path);
        }

        public void InitPatchSubRegistry(PatchArea patchArea)
        {
            var subRegistry = new DictPatchSubRegistry<T>();
            patchArea.AddCustomPatchRegistry<T>(subRegistry);
        }

        public void CreateDefaultFile(PatchArea patchArea)
        {
            var file = patchArea.GetCustomPatchRegistry<DictPatchSubRegistry<T>, T>().CreateFile();
            patchArea.DefaultStats.Set(Path.String, file);
        }
    }
}