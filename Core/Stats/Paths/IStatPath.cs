using Hopper.Core.FS;
using Hopper.Core.Registries;

namespace Hopper.Core.Stats
{
    public interface IStatPath<out T> where T : File
    {
        string String { get; }
        T Path(StatManager sm);
        T GetDefault(PatchArea patchArea);
    }
}