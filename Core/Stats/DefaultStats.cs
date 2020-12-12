using Hopper.Core.FS;

namespace Hopper.Core.Stats
{
    public class DefaultStats : IPatch
    {
        public StatManager statManager;
        public Registry Registry { get; private set; }

        public DefaultStats(Registry registry)
        {
            this.Registry = registry;
            this.statManager = new StatManager();
        }

        public void PatchKindRegistry(int kindId)
        {
            this.Registry.EntityFactoryPatch.Add(kindId, this);
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

        public DefaultStats SetAtIndex(IStatPath<ArrayFile> statPath, int index, int value)
        {
            statManager.GetRaw(statPath.String, statPath.GetDefault(Registry))[index] = value;
            return this;
        }
    }
}