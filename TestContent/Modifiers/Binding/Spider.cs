using System.Runtime.Serialization;
using Hopper.Core;
using Hopper.Core.ActingNS;
using Hopper.Core.Components.Basic;
using Hopper.Core.WorldNS;
using Hopper.Shared.Attributes;

namespace Hopper.TestContent.BindingNS
{
    [EntityType]
    public static class Spider
    {
        public static EntityFactory Factory;
        
        private static readonly JoinedAction BindMoveAction = Action.Join(Binding.Action, Moving.Action);

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
            Binding.AddTo(subject, Layers.REAL, BoundEntityModifierDefault.Hookable);
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