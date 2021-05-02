using System.Runtime.Serialization;
using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Shared.Attributes;

namespace Hopper.TestContent.Bind
{
    [EntityType]
    public static class Spider
    {
        public static EntityFactory Factory;
        
        private static readonly DirectedAction BindAction = Action.FromActivateable(Binding.Index);
        private static readonly DirectedAction MoveAction = Action.FromActivateable(Moving.Index);
        private static readonly DirectedAction BindMoveAction = Action.CreateJoinedDirected(BindAction, MoveAction);

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
            Binding.AddTo(subject, Layer.REAL);
        }

        public static void InitComponents(Entity subject)
        {
            SequentialMobBase.InitComponents(subject);
            subject.GetBinding().DefaultPreset();
        }

        public static void Retouch(Entity subject)
        {
            SequentialMobBase.Retouch(subject);
        }
    }
}