using Chains;

namespace Core.Stats
{
    public class StatEvent<T> : EventBase where T : IStatFile<T>
    {
        public T file;
    }
}