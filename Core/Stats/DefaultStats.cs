using Hopper.Core.FS;

namespace Hopper.Core.Stats
{
    public class DefaultStats
    {
        public StatManager StatManager { get; private set; }

        public DefaultStats()
        {
            StatManager = new StatManager();
        }

        public DefaultStats Set<T>(IStatPath<T> statPath, T value)
            where T : File, new()
        {
            StatManager.GetRaw(statPath.String, value);
            return this;
        }
    }
}