using Hopper.Utils.FS;
using Hopper.Core.Registries;

namespace Hopper.Core.Stats
{
    public class DefaultStats : IPatchable
    {
        public StatManager statManager;
        public PatchArea PatchArea { get; private set; }

        public DefaultStats(PatchArea patchArea)
        {
            this.PatchArea = patchArea;
            this.statManager = new StatManager();
        }

        public DefaultStats Set<T>(IStatPath<T> statPath, T value)
            where T : File, new()
        {
            statManager.GetRawLazy(statPath.String, value);
            return this;
        }

        public DefaultStats Set<T>(string path, T value)
            where T : File, new()
        {
            statManager.GetRawLazy(path, value);
            return this;
        }

        public DefaultStats SetAtIndex(IStatPath<DictFile> statPath, int index, int value)
        {
            statManager.GetRawLazy(statPath.String, statPath.GetDefault(PatchArea))[index] = value;
            return this;
        }
    }
}