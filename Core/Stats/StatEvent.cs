using Chains;
using Core.FS;

namespace Core.Stats
{
    public class StatEvent<T> : EventBase where T : File
    {
        public T file;
    }
}