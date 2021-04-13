using Hopper.Utils;

namespace Hopper.Core.Components
{
    public interface IComponent : ICopyable {}
    public interface ITag : IComponent {}
    public interface IBehavior : IComponent {}
}