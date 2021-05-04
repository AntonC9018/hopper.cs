using System.Runtime.Serialization;
using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Shared.Attributes;
using static Hopper.Core.Action;

namespace Hopper.TestContent.Bind
{
    [EntityType]
    public static class Spider
    {
        public static EntityFactory Factory;
        
        private static readonly ActivatingAction<Binding> BindAction = FromActivateable(Binding.Index);
        private static readonly ActivatingAction<Moving> MoveAction = FromActivateable(Moving.Index);
        private static readonly JoinedAction BindMoveAction = Join(BindAction, MoveAction);

        private static readonly Step[] Steps = new Step[]
        {
            new Step
            {
                action = BindMoveAction,
                movs = Movs.Diagonal,
                successFunction = acting => new Result
                {
                    success = acting.actor.IsCurrentlyBinding()
                }
            },
            new Step
            {
                action = null,
                successFunction = acting => new Result
                {
                    success = !acting.actor.IsCurrentlyBinding()
                },
            }
        };

        public static void AddComponents(Entity subject)
        {
            SequentialMobBase.AddComponents(subject, Algos.EnemyAlgo, Steps);
            Binding.AddTo(subject, Layer.REAL, BoundEntityModifier.DefaultHookable);
        }

        public static void InitComponents(Entity subject)
        {
            SequentialMobBase.InitComponents(subject);
            subject.GetBinding().DefaultPreset();
        }

        public static void Retouch(EntityFactory factory)
        {
            SequentialMobBase.Retouch(factory);
        }
    }
}