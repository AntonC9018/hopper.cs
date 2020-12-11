using Hopper.Core.FS;

namespace Hopper.Core.Stats
{
    public interface IStatPath<out T> where T : File
    {
        string String { get; }
        T Path(StatManager sm);
        T GetDefault(Registry registry);
    }

    // these are either static or created uniquely for kinds
    public class StatPath<T> : IStatPath<T>
        where T : File, new()
    {
        public string String { get; protected set; }
        public readonly System.Func<Registry, T> CreateDefaultFile;

        public StatPath(string path, System.Func<Registry, T> CreateDefaultFile)
        {
            this.String = path;
            this.CreateDefaultFile = CreateDefaultFile;
        }

        public T Path(StatManager sm)
        {
            return sm.GetLazy<T>(this);
        }

        public void SetDefaultFile(Registry registry)
        {
            registry.DefaultStats.statManager.GetLazy<T>(String, CreateDefaultFile(registry));
        }

        public T GetDefault(Registry registry)
        {
            // we know that at this point the stat has been initialized since it is
            // called per Registry in the startup function
            return registry.DefaultStats.statManager.GetUnsafe<T>(String);
        }
    }

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

        public T Path(StatManager sm)
        {
            return sm.GetLazy<T>(this);
        }

        public T GetDefault(Registry registry)
        {
            return defaultFile;
        }
    }
}