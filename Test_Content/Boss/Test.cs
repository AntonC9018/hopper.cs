using Hopper.Core;
using Hopper.Core.Behaviors;
using Hopper.Utils.Vector;

namespace Hopper.Test_Content
{
    public class TestBoss : Entity
    {
        private int m_whelpCount = 0;
        private int m_whelpMax = 3;

        private static Action AttackMoveAction = new CompositeAction(
            new BehaviorAction<Attacking>(), new BehaviorAction<Moving>()
        );

        private static Action SpawnAction = new SimpleAction(
            (e, a) => Spawn((TestBoss)e, a)
        );

        private static Step[] Steps = new[]
        {
            new Step
            {
                action = AttackMoveAction,
                movs = Movs.Basic
            },
            new Step
            {
                action = Laser.LaserShootAction,
                movs = Movs.Basic
            },
            new Step
            {
                repeat = 3,
            },
            new Step
            {
                action = SpawnAction,
                movs = Movs.Basic
            },
        };

        private static void Spawn(TestBoss entity, Action action)
        {
            int toSpawn = entity.m_whelpMax - entity.m_whelpCount;
            for (int i = 0; i < toSpawn; i++)
            {
                var pos = entity.Pos - action.direction * (i + 1);
                if (entity.World.Grid.IsOutOfBounds(pos) == false)
                {
                    var whelp = entity.World.SpawnEntity(Whelp.Factory, pos, action.direction);
                    whelp.DieEvent += () => entity.m_whelpCount--;
                }
                entity.m_whelpCount++;
            }
        }

        private static void TurnToPlayer(Entity entity)
        {
            var player = entity.GetClosestPlayer();
            var diff = player.Pos - entity.Pos;
            var sign = diff.Sign();
            var abs = diff.Abs();
            if (abs.x > abs.y)
            {
                entity.Orientation = new IntVector2(sign.x, 0);
            }
            if (abs.y > abs.x)
            {
                entity.Orientation = new IntVector2(0, sign.y);
            }
        }

        private static Retoucher TurnToPlayerRetoucher = Retoucher
            .SingleHandlered(Acting.Success, ev => TurnToPlayer(ev.actor));

        public class Whelp : Entity
        {
            private static Step[] Steps = new[]
            {
                new Step
                {
                    repeat = 1
                },
                new Step
                {
                    action = AttackMoveAction,
                    movs = Movs.Basic
                },
                new Step
                {
                    repeat = 1
                },
                new Step
                {
                    action = AttackMoveAction,
                    movs = Movs.Adjacent
                }
            };

            public static readonly EntityFactory<Whelp> Factory = new EntityFactory<Whelp>()
                .AddBehavior<Acting>(new Acting.Config(Algos.EnemyAlgo))
                .AddBehavior<Attacking>()
                .AddBehavior<Attackable>()
                .AddBehavior<Moving>()
                .AddBehavior<Displaceable>()
                .AddBehavior<Damageable>(new Damageable.Config(1))
                .Retouch(Hopper.Core.Retouchers.Skip.NoPlayer)
                .Retouch(Hopper.Core.Retouchers.Skip.BlockedMove)
                // .Retouch(Core.Retouchers.Reorient.OnActionSuccess)
                .Retouch(TurnToPlayerRetoucher)
                .AddBehavior<Sequential>(new Sequential.Config(Steps));
        }

        public static readonly EntityFactory<TestBoss> Factory = new EntityFactory<TestBoss>()
            .AddBehavior<Acting>(new Acting.Config(Algos.EnemyAlgo))
            .AddBehavior<Attacking>()
            .AddBehavior<Attackable>()
            .AddBehavior<Moving>()
            .AddBehavior<Displaceable>()
            .AddBehavior<Damageable>(new Damageable.Config(5))
            .Retouch(Hopper.Core.Retouchers.Skip.NoPlayer)
            .Retouch(Hopper.Core.Retouchers.Skip.BlockedMove)
            // .Retouch(Core.Retouchers.Reorient.OnActionSuccess)
            .Retouch(TurnToPlayerRetoucher)
            .AddBehavior<Sequential>(new Sequential.Config(Steps));
    }
}