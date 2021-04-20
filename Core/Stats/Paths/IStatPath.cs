using Hopper.Utils.FS;

namespace Hopper.Core.Stat
{
    public interface IStatPath<out T>
    {
        T Path(Stats sm);
    }
}