using Hopper.Core;
using Hopper.Core.Targeting;
using Hopper.Utils.Vector;

namespace Hopper.Test_Content
{
    public interface IAnonShooting
    {
        ShootingInfo ShootAnon(IWorldSpot spot, IntVector2 direction);
    }
    public interface INormalShooting
    {
        ShootingInfo Shoot(Entity entity, IntVector2 direction);
    }
    public interface IAnyShooting : IAnonShooting, INormalShooting
    {
    }
}