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
        private static Attack DefaultAttack(Registry reg) =>
            new Attack
            {
                power = 1,
                sourceId = AttackSource.GetId(reg),
                damage = 2,
                pierce = 5
            };
        public static readonly Push.Source PushSource = new Push.Source { resistance = 1 };
        private static Push DefaultPush(Registry reg) =>
            new Push
            {
                sourceId = PushSource.GetId(reg),
                power = 1,
                distance = 1,
                pierce = 1
            };
        private static Layer TargetedLayer = Layer.REAL;
        private static Layer SkipLayer = Layer.WALL;

        private static IAnonShooting DefaultShooting = new AnonShooting(
            TargetedLayer, SkipLayer, DefaultAttack, DefaultPush, false);

        public static void Shoot(IWorldSpot spot, IntVector2 dir)
        {
            var shooting_info = DefaultShooting.ShootAnon(spot, dir);
            var laser_info = new LaserInfo(dir, spot.Pos + dir, shooting_info.last_checked_pos - dir);
            EventPath.Fire(spot.World, laser_info);
        }

        public static readonly SimpleAction LaserShootAction = new SimpleAction(
            (e, a) => Laser.Shoot(e, a.direction)
        );
    }
}