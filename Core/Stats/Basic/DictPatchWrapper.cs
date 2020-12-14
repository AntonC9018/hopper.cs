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

        public void InitPatchSubRegistry(Repository repository)
        {
            var subRegistry = new DictPatchSubRegistry<T>();
            repository.AddCustomPatchRegistry<T>(subRegistry);
        }

        public void CreateDefaultFile(Repository repository)
        {
            var file = repository.GetCustomPatchRegistry<DictPatchSubRegistry<T>, T>().CreateFile();
            repository.DefaultStats.Set(Path.String, file);
        }
    }
}