using Hopper.Utils.FS;
using Hopper.Core.Registries;

namespace Hopper.Core.Stat
{
    public class SimpleStatPath<T> : IStatPath<T>
        where T : File, new()
    {
        public string String { get; protected set; }
        public readonly T defaultFile;

        public SimpleStatPath(string path)
        {
            this.String = path;
            this.defaultFile = new T();
        }

        public SimpleStatPath(string path, T defaultFile)
        {
            this.String = path;
            this.defaultFile = defaultFile;
        }

        public T Path(Stats sm)
        {
            return sm.GetLazy<T>(this);
        }

        public T GetDefault(PatchArea registry)
        {
            return defaultFile;
        }
    }
}