using Hopper.Core.FS;
using Hopper.Core.Registry;

namespace Hopper.Core.Stats
{
    public class DefaultStats : IPatchable
    {
        public StatManager statManager;
        public PatchArea Repository { get; private set; }

        public DefaultStats(PatchArea patchArea)
        {
            this.Repository = patchArea;
            this.statManager = new StatManager();
        }

        public DefaultStats Set<T>(IStatPath<T> statPath, T value)
            where T : File, new()
        {
            statManager.GetRaw(statPath.String, value);
            return this;
        }

        public DefaultStats Set<T>(string path, T value)
            where T : File, new()
        {
            statManager.GetRaw(path, value);
            return this;
        }

        public DefaultStats SetAtIndex(IStatPath<DictFile> statPath, int index, int value)
        {
            statManager.GetRaw(statPath.String, statPath.GetDefault(Repository))[index] = value;
            return this;
        }
    }
}