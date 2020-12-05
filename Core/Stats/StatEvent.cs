using Chains;
using Hopper.Core.FS;

namespace Hopper.Core.Stats
{
    public class StatEvent<T> : EventBase where T : File
    {
        public T file;
    }
}