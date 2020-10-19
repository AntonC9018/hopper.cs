namespace Core.Stats
{
    public interface IStatPath<out T> where T : StatFile, new()
    {
        string String { get; }
        T DefaultFile { get; }
        T Path(StatManager sm);
    }

    public class StatPath<T> : IStatPath<T> where T : StatFile, new()
    {
        public string String { get; private set; }
        public T DefaultFile { get; private set; }

        public StatPath(string path)
        {
            this.String = path;
            this.DefaultFile = new T();
        }

        public StatPath(string path, T defaultFile)
        {
            this.String = path;
            this.DefaultFile = defaultFile;
        }

        public T Path(StatManager sm)
        {
            return (T)sm.GetFileLazy(String, DefaultFile);
        }
    }
}