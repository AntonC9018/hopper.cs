using Hopper.Core.Registries;

namespace Hopper.Core
{
    public interface IFactory<out T> : IKind, IPostPatch
    {
        T Instantiate();
    }
}