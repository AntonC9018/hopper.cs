using Hopper.Utils;
using Hopper.Utils.Vector;

namespace Hopper.Core.Components
{
    public interface IComponent {}
    public interface ITag : IComponent {}
    public interface IBehavior : IComponent {}

    public interface IStandartActivateable
    {
        bool Activate(Entity entity, IntVector2 direction);
    }
}