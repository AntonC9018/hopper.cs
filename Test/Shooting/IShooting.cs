using Core;
using Core.Targeting;
using Core.Utils.Vector;

namespace Test
{
    public interface IAnonShooting
    {
        ShootingInfo ShootAnon(IWorldSpot spot, IntVector2 direction);
    }
    public interface INormalShooting
    {
        ShootingInfo Shoot(Entity entity, Action action);
    }
    public interface IAnyShooting : IAnonShooting, INormalShooting
    {
    }
}