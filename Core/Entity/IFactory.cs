using Hopper.Core.Registry;

namespace Hopper.Core
{
    public interface IFactory<out T> : IKind, IPostPatch
    {
        T Instantiate();
    }
}