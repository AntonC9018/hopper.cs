using Hopper.Core;
using Hopper.Core.Behaviors.Basic;
using Hopper.Core.Retouchers;
using Hopper.Test_Content;
using Hopper.Utils.Vector;

namespace Hopper.Test_Content.Boss
{
    public class TestBoss : Entity
    {
        public static readonly Retoucher TurnToPlayerRetoucher;
        public static EntityFactory<TestBoss> Factory;
        private static readonly Action AttackMoveAction;
        private static readonly Action SpawnAction;
        private static readonly Step[] Steps;

        private int m_whelpCount = 0;
        private static int WhelpMax = 3;

        private static void Spawn(TestBoss entity, Action action)
        {
            int toSpawn = WhelpMax - entity.m_whelpCount;
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


        public class Whelp : Entity
        {
            public static EntityFactory<Whelp> Factory;
            private static readonly Step[] Steps;

            static Whelp()
            {
                Steps = new[]
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
            }

            public static EntityFactory<Whelp> CreateFactory()
            {
                return new EntityFactory<Whelp>()
                    .AddBehavior(Acting.Preset(new Acting.Config(Algos.EnemyAlgo)))
                    .AddBehavior(Attacking.Preset)
                    .AddBehavior(Attackable.DefaultPreset)
                    .AddBehavior(Moving.Preset)
                    .AddBehavior(Displaceable.DefaultPreset)
                    .AddBehavior(Damageable.Preset)
                    .Retouch(Skip.NoPlayer)
                    .Retouch(Skip.BlockedMove)
                    // .Retouch(Core.Retouchers.Reorient.OnActionSuccess)
                    .Retouch(TurnToPlayerRetoucher)
                    .AddBehavior(Sequential.Preset(new Sequential.Config(Steps)));
            }
        }

        static TestBoss()
        {
            AttackMoveAction = new CompositeAction(
                new BehaviorAction<Attacking>(), new BehaviorAction<Moving>());
            SpawnAction = new SimpleAction(
                (e, a) => Spawn((TestBoss)e, a));
            Steps = new[]
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
            TurnToPlayerRetoucher = Retoucher.SingleHandlered(Acting.Success, ev => TurnToPlayer(ev.actor));
        }


        public static EntityFactory<TestBoss> CreateFactory()
        {
            return new EntityFactory<TestBoss>()
                .AddBehavior(Acting.Preset(new Acting.Config(Algos.EnemyAlgo)))
                .AddBehavior(Attacking.Preset)
                .AddBehavior(Attackable.DefaultPreset)
                .AddBehavior(Moving.Preset)
                .AddBehavior(Displaceable.DefaultPreset)
                .AddBehavior(Damageable.Preset)
                .Retouch(Skip.NoPlayer)
                .Retouch(Skip.BlockedMove)
                // .Retouch(Core.Retouchers.Reorient.OnActionSuccess)
                .Retouch(TurnToPlayerRetoucher)
                .AddBehavior(Sequential.Preset(new Sequential.Config(Steps)));
        }
    }
}