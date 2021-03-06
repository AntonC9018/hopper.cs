using Hopper.Core;
using Hopper.Core.ActingNS;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Core.WorldNS;
using Hopper.Shared.Attributes;
using Hopper.TestContent.LaserNS;
using Hopper.Utils.Vector;
using static Hopper.Core.ActingNS.Action;

namespace Hopper.TestContent.Boss
{
    public partial class TestBossComponent : IComponent, IStandartActivateable
    {
        public static int WhelpCap = 3;
        public int whelpCount;


        public bool ShouldSpawn()
        {
            return whelpCount < WhelpCap;
        }

        public bool Activate(Entity actor, IntVector2 direction)
        {
            if (!ShouldSpawn())
                return true;

            int amountToSpawn = WhelpCap - whelpCount;
            var transform = actor.GetTransform();
            IntVector2 nextPosition = transform.position;

            while (whelpCount < WhelpCap)
            {
                nextPosition -= direction;

                if (World.Global.Grid.IsOutOfBounds(nextPosition))
                    break;

                if (World.Global.Grid.HasNoUndirectedTransformAt(nextPosition, Layers.REAL))
                {
                    World.Global.SpawnEntity(RobotSummon.Factory, nextPosition, direction);
                    whelpCount++;
                }
            }

            return true;
        }
    }

    [EntityType]
    public static class Robot
    {
        public static EntityFactory Factory;
        
        private static readonly CompositeAction AttackMoveAction = Compose(Attacking.Action, Moving.Action);

        private static readonly Step[] Steps = new[]
        {
            new Step
            {
                action = AttackMoveAction,
                movs = Movs.Basic
            },
            new Step
            {
                action = Laser.ShootAction,
                movs = Movs.Basic
            },
            new Step
            {
                repeat = 3,
            },
            new Step
            {
                action = TestBossComponent.Action, // spawn action
                movs = Movs.Basic
            },
        };


        public static void AddComponents(Entity subject)
        {
            SequentialMobBase.AddComponents(subject, Algos.EnemyAlgo, Steps);
        }

        public static void InitComponents(Entity subject)
        {
            SequentialMobBase.InitComponents(subject);
        }

        public static void Retouch(EntityFactory factory)
        {
            SequentialMobBase.Retouch(factory);
        }
    }

    
    [EntityType]
    public static class RobotSummon
    {
        public static EntityFactory Factory;

        private static readonly CompositeAction AttackMoveAction = Compose(Attacking.Action, Moving.Action);

        private static readonly Step[] Steps = new[]
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

        public static void AddComponents(Entity subject)
        {
            SequentialMobBase.AddComponents(subject, Algos.EnemyAlgo, Steps);
        }

        public static void InitComponents(Entity subject)
        {
            SequentialMobBase.InitComponents(subject);
        }

        public static void Retouch(EntityFactory factory)
        {
            SequentialMobBase.Retouch(factory);
        }
    }
}