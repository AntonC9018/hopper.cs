using Hopper.Core.WorldNS;

namespace Hopper.Core.ActingNS
{
    public static partial class Movs
    {
        public class Relativeness
        {
            public bool gx, gy, lx, ly;
        }
        public static Relativeness CalculateRelativeness(Transform actor, Transform player)
        {
            return new Relativeness
            {
                gx = player.position.x > actor.position.x,
                gy = player.position.y > actor.position.y,
                lx = actor.position.x > player.position.x,
                ly = actor.position.y > player.position.y
            };
        }
    }
}