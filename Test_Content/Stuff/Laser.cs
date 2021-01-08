using Hopper.Core;
using Hopper.Core.Stats.Basic;
using Hopper.Core.Targeting;
using Hopper.Utils.Vector;

namespace Hopper.Test_Content
{
    public class LaserInfo
    {
        public IntVector2 direction;
        public IntVector2 pos_start;
        public IntVector2 pos_end;

        public LaserInfo(IntVector2 direction, IntVector2 pos_start, IntVector2 pos_end)
        {
            this.direction = direction;
            this.pos_start = pos_start;
            this.pos_end = pos_end;
        }
    }

    public static class Laser
    {
        public static readonly WorldEventPath<LaserInfo> EventPath = new WorldEventPath<LaserInfo>();
        public static readonly Attack.Source AttackSource = new Attack.Source { resistance = 1 };
        public static readonly Push.Source PushSource = new Push.Source { resistance = 1 };

        private static readonly Attack DefaultAttack =
            new Attack
            {
                power = 1,
                sourceId = AttackSource.Id,
                damage = 2,
                pierce = 5
            };
        private static readonly Push DefaultPush =
            new Push
            {
                sourceId = PushSource.Id,
                power = 1,
                distance = 1,
                pierce = 1
            };
        private static readonly TargetLayers TargetLayers = new TargetLayers
        {
            targeted = Layer.REAL,
            skip = Layer.WALL
        };

        private static readonly AnonShooting DefaultShooting = new AnonShooting(
            TargetLayers, DefaultAttack, DefaultPush, false);

        public static void Shoot(IWorldSpot spot, IntVector2 dir)
        {
            var shooting_info = DefaultShooting.ShootAnon(spot, dir);
            var laser_info = new LaserInfo(dir, spot.Pos + dir, shooting_info.last_checked_pos - dir);
            EventPath.Fire(spot.World, laser_info);
        }

        public static readonly DirectedAction LaserShootAction =
            Action.CreateSimple(Laser.Shoot, DefaultShooting.Predict);
    }
}