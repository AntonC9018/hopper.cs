using Hopper.Utils.Vector;

namespace Hopper.Core.Components
{
    public interface IStandartActivateable
    {
        bool Activate(Entity entity, IntVector2 direction);
    }
}