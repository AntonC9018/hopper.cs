using Hopper.Utils.Chains;
using Hopper.Utils.FS;

namespace Hopper.Core.Stats
{
    public class StatContext<T> : ContextBase where T : File
    {
        public T file;
    }
}