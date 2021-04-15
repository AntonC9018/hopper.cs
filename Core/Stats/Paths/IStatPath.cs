using Hopper.Utils.FS;

namespace Hopper.Core.Stats
{
    public interface IStatPath<out T>
    {
        T Path(Stats sm);
    }
}