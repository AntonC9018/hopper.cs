using Core;
using Core.Stats.Basic;
using Core.Targeting;
using Core.Utils.Vector;

namespace Test
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

        private static Attack.Source AttackSource = new Attack.Source();
        private static Attack DefaultAttack = new Attack
        {
            power = 1,
            sourceId = AttackSource.Id,
            damage = 2,
            pierce = 5
        };
        private static Push.Source PushSource = new Push.Source();
        private static Push DefaultPush = new Push
        {
            power = 1,
            sourceId = PushSource.Id,
            distance = 1
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